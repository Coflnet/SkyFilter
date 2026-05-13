using System.Collections.Generic;
using Coflnet.Sky.Core;
using fNbt.Tags;
using NUnit.Framework;

namespace Coflnet.Sky.Filter;

public class FilterControllerTests
{
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