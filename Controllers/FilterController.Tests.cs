using System.Collections.Generic;
using Coflnet.Sky.Core;
using fNbt.Tags;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Coflnet.Sky.Filter;

public class FilterControllerTests
{
    // a request carrying the pre-flattened nbt as plain json (nbtData can not be sent over json).
    // YOUNG_DRAGON_LEGGINGS with color 31:0:48 == hex 1F0030, which is a crystal color.
    private const string CrystalFlatNbtPayload = @"{""filters"":{""ExoticColor"":""Fairy+Crystal""},""auction"":{""uuid"":""d3b9683516a149bdbc1896ddc971136d"",""tag"":""YOUNG_DRAGON_LEGGINGS"",""itemCreatedAt"":""2020-01-31T14:11:00"",""category"":""ARMOR"",""tier"":""LEGENDARY"",""bin"":true,""nbtData"":{""data"":{""color"":""31:0:48""}},""flatNbt"":{""color"":""31:0:48"",""uid"":""176399aaa739"",""uuid"":""0a8a3030-db17-4022-b992-176399aaa739""}}}";

    [Test]
    public void FlatNbtIsDeserializedFromJson()
    {
        // regression: the FlatenedNBT getter returns a fresh empty dictionary when nothing is set,
        // so without ObjectCreationHandling.Replace Newtonsoft populated a throwaway and dropped the values.
        var query = JsonConvert.DeserializeObject<Controllers.FilterController.FilterQuery>(CrystalFlatNbtPayload);
        Assert.That(query.Auction.FlatenedNBT, Is.Not.Null);
        Assert.That(query.Auction.FlatenedNBT.GetValueOrDefault("color"), Is.EqualTo("31:0:48"));
    }

    [Test]
    public void ExoticColorFilterMatchesCrystalColorFromFlatNbtPayload()
    {
        var query = JsonConvert.DeserializeObject<Controllers.FilterController.FilterQuery>(CrystalFlatNbtPayload);
        var engine = new FilterEngine(new MockNbt());
        // mirrors FilterController.MatchesFilter
        var matches = engine.GetMatchExpression(query.Filters).Compile()(query.Auction);
        Assert.That(matches, Is.True, "1F0030 is a crystal color and should match the Fairy+Crystal filter");
    }

    [Test]
    public void ApiSaveAuctionFallsBackToFlattenedNbtFromNbtData()
    {
        var auction = new Controllers.FilterController.ApiSaveAuction
        {
            NbtData = new NbtData(new NbtCompound
            {
                new NbtCompound("tag")
                {
                    new NbtCompound("ExtraAttributes")
                    {
                        new NbtCompound("engine")
                        {
                            new NbtString("id", "TITANIUM_DRILL_ENGINE")
                        }
                    }
                }
            })
        };

        Assert.That(auction.FlatenedNBT, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(auction.FlatenedNBT.GetValueOrDefault("engine.id"), Is.EqualTo("TITANIUM_DRILL_ENGINE"));
            Assert.That(auction.FlatenedNBT.ContainsKey("id"), Is.False);
        });
    }
}