using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Coflnet.Sky.Core;
using Coflnet.Sky.Core.Services;
using Coflnet.Sky.Filter.Tests;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace Coflnet.Sky.Filter
{
    public class ExoticColorFilterTests
    {
        private readonly FilterEngine filterEngine = new(new MockNbt());

        private async Task<ExoticColorFilter> CreateLoadedFilter()
        {
            var provider = new ServiceCollection()
                .AddSingleton<HttpClient>()
                .AddSingleton(typeof(ILogger<>), typeof(NullLogger<>))
                .AddSingleton<HypixelItemService>()
                .AddSingleton<ExoticColorService>()
                .BuildServiceProvider();
            var filter = new ExoticColorFilter();
            await filter.LoadData(provider);
            return filter;
        }

        private bool MatchInMemory(ExoticColorFilter filter, string value, SaveAuction auction)
            => filter.GetExpression(new FilterArgs(new Dictionary<string, string>() { { "ExoticColor", value } }, false, filterEngine))
                .Compile()(auction);

        private static SaveAuction Auction(string tag, string hex)
        {
            // NBT stores the color as an "r:g:b" string; the filter converts it back to hex.
            var r = System.Convert.ToInt32(hex.Substring(0, 2), 16);
            var g = System.Convert.ToInt32(hex.Substring(2, 2), 16);
            var b = System.Convert.ToInt32(hex.Substring(4, 2), 16);
            return new SaveAuction()
            {
                Tag = tag,
                FlatenedNBT = new() { { "color", $"{r}:{g}:{b}" } },
                ItemCreatedAt = new System.DateTime(2023, 1, 1)
            };
        }

        [Test]
        public async Task FairyColorMatchesViaService()
        {
            var filter = await CreateLoadedFilter();
            Assert.That(MatchInMemory(filter, "Fairy", Auction("SUPERIOR_DRAGON_CHESTPLATE", "FF00FF")), Is.True, "fairy color should match");
            Assert.That(MatchInMemory(filter, "Fairy", Auction("SUPERIOR_DRAGON_CHESTPLATE", "123456")), Is.False, "random color should not match fairy");
        }

        [Test]
        public async Task CrystalColorMatchesViaService()
        {
            var filter = await CreateLoadedFilter();
            Assert.That(MatchInMemory(filter, "Crystal", Auction("SUPERIOR_DRAGON_CHESTPLATE", "1F0030")), Is.True, "crystal color should match");
            Assert.That(MatchInMemory(filter, "Crystal", Auction("SUPERIOR_DRAGON_CHESTPLATE", "FF00FF")), Is.False, "fairy color should not match crystal");
        }

        [Test]
        public async Task AnyMatchesExoticButNotFairyOrCrystal()
        {
            var filter = await CreateLoadedFilter();
            Assert.That(MatchInMemory(filter, "Any:FFFFFF", Auction("SUPERIOR_DRAGON_CHESTPLATE", "123456")), Is.True, "unusual color should count as exotic");
            Assert.That(MatchInMemory(filter, "Any:FFFFFF", Auction("SUPERIOR_DRAGON_CHESTPLATE", "FF00FF")), Is.False, "fairy color is not plain exotic");
            Assert.That(MatchInMemory(filter, "Any:FFFFFF", Auction("SUPERIOR_DRAGON_CHESTPLATE", "1F0030")), Is.False, "crystal color is not plain exotic");
        }

        [Test]
        public async Task ExoticMatchesOnlyPlainExotic()
        {
            var filter = await CreateLoadedFilter();
            Assert.That(MatchInMemory(filter, "Exotic:FFFFFF", Auction("SUPERIOR_DRAGON_CHESTPLATE", "123456")), Is.True, "unusual color is exotic");
            Assert.That(MatchInMemory(filter, "Exotic:FFFFFF", Auction("FAIRY_HELMET", "000000")), Is.False, "spook color is not plain exotic");
            Assert.That(MatchInMemory(filter, "Exotic:FFFFFF", Auction("SUPERIOR_DRAGON_CHESTPLATE", "FF00FF")), Is.False, "fairy color is not plain exotic");
        }

        [Test]
        public async Task PerColorTypeOptionsClassifyViaService()
        {
            var filter = await CreateLoadedFilter();
            // Undyed
            Assert.That(MatchInMemory(filter, "Undyed", Auction("SUPERIOR_DRAGON_CHESTPLATE", "A06540")), Is.True);
            Assert.That(MatchInMemory(filter, "Undyed", Auction("SUPERIOR_DRAGON_CHESTPLATE", "123456")), Is.False);
            // Spook (fairy item wearing a spook color)
            Assert.That(MatchInMemory(filter, "Spook", Auction("FAIRY_HELMET", "000000")), Is.True);
            Assert.That(MatchInMemory(filter, "Spook", Auction("FAIRY_HELMET", "123456")), Is.False);
            // Glitched (other-glitched mapping)
            Assert.That(MatchInMemory(filter, "Glitched", Auction("SHARK_SCALE_CHESTPLATE", "FFDC51")), Is.True);
            Assert.That(MatchInMemory(filter, "Glitched", Auction("SHARK_SCALE_CHESTPLATE", "123456")), Is.False);
            // Plain Fairy (fairy color that is not an OG fairy color)
            Assert.That(MatchInMemory(filter, "Plain Fairy", Auction("SUPERIOR_DRAGON_CHESTPLATE", "FF007F")), Is.True);
            Assert.That(MatchInMemory(filter, "Plain Fairy", Auction("SUPERIOR_DRAGON_CHESTPLATE", "123456")), Is.False);
        }

        [Test]
        public void OptionsAreCleanLabels()
        {
            var filter = new ExoticColorFilter();
            var options = filter.OptionsGet(new OptionValues(new Dictionary<string, List<string>>
            {
                ["color"] = new() { "0", "16711680", "255" }
            })).Select(o => o.ToString()).ToList();

            // every ColorType the service can return must be reachable through some option
            foreach (var expected in new[] { "Any", "Exotic", "Original", "Crystal", "Fairy", "Plain Fairy", "OG Fairy", "Undyed", "Glitched", "Spook" })
                Assert.That(options, Does.Contain(expected));

            // plain labels only, no color lists riding along in the value
            foreach (var option in options)
                Assert.That(option, Does.Not.Contain(":"), $"{option} should be a plain label");
        }

        [Test]
        public void OptionsEmptyWithoutColors()
        {
            var filter = new ExoticColorFilter();
            var options = filter.OptionsGet(new OptionValues(new Dictionary<string, List<string>>()));
            Assert.That(options, Is.Empty, "items without colors should not offer exotic color options");
        }

        [Test]
        public async Task DyedItemsAreNotFlaggedExotic()
        {
            var filter = await CreateLoadedFilter();

            var exotic = Auction("SUPERIOR_DRAGON_CHESTPLATE", "123456");
            Assert.That(MatchInMemory(filter, "Any:FFFFFF", exotic), Is.True, "undyed exotic color should match");

            // same color but reached via a dye -> not a natural exotic color
            exotic.FlatenedNBT["dye_item"] = "DYE_HOLLOW";
            Assert.That(MatchInMemory(filter, "Any:FFFFFF", exotic), Is.False, "dyed item should be excluded");
        }

        [Test]
        public async Task ExplicitHexStillMatchesExactColor()
        {
            var filter = await CreateLoadedFilter();
            // A raw hex value (no category label) keeps the plain color-value match.
            Assert.That(MatchInMemory(filter, "ABCDEF", Auction("SUPERIOR_DRAGON_CHESTPLATE", "ABCDEF")), Is.True);
            Assert.That(MatchInMemory(filter, "ABCDEF", Auction("SUPERIOR_DRAGON_CHESTPLATE", "123456")), Is.False);
        }

        // ---- database path (TargetsDB) : the color set is rebuilt from constants / carried default ----

        private static SaveAuction DbAuction(long colorValue, bool dyed = false)
        {
            var lookups = new List<NBTLookup> { new NBTLookup(KeyMapNbt.ColorKey, (int)colorValue) };
            if (dyed)
                lookups.Add(new NBTLookup(KeyMapNbt.DyeKey, 1));
            return new SaveAuction() { NBTLookup = lookups.ToArray() };
        }

        private bool MatchDb(ExoticColorFilter filter, string value, SaveAuction auction, Dictionary<string, string> extraFilters = null)
        {
            var filters = new Dictionary<string, string>() { { "ExoticColor", value } };
            if (extraFilters != null)
                foreach (var kv in extraFilters)
                    filters[kv.Key] = kv.Value;
            var args = new FilterArgs(filters, true, null)
            {
                NbtIntance = new KeyMapNbt()
            };
            return filter.GetExpression(args).Compile()(auction);
        }

        [Test]
        public async Task DbFairyRebuildsColorSetFromConstants()
        {
            var filter = await CreateLoadedFilter();
            Assert.That(MatchDb(filter, "Fairy", DbAuction(filter.FromHex("FF00FF"))), Is.True, "fairy color should match on db");
            Assert.That(MatchDb(filter, "Fairy", DbAuction(filter.FromHex("123456"))), Is.False, "random color should not match fairy on db");
        }

        [Test]
        public async Task DbAnyExcludesKnownNormalColors()
        {
            var filter = await CreateLoadedFilter();
            Assert.That(MatchDb(filter, "Any", DbAuction(filter.FromHex("123456"))), Is.True, "unusual color is exotic on db");
            Assert.That(MatchDb(filter, "Any", DbAuction(filter.FromHex("A06540"))), Is.False, "undyed color is a known normal color");
            Assert.That(MatchDb(filter, "Any", DbAuction(filter.FromHex("FF00FF"))), Is.False, "fairy color is a known normal color");
            Assert.That(MatchDb(filter, "Any", DbAuction(filter.FromHex("1F0030"))), Is.False, "crystal color is a known normal color");
        }

        [Test]
        public async Task DbAnyExcludesItemDefaultColorResolvedFromItemId()
        {
            // seed a fake item whose default color is 123456 (not otherwise a "normal" color)
            var (filter, service, details) = await CreateLoadedFilterWithItemData();
            details.TagLookup["TEST_ITEM"] = 999;
            SeedItems(service, new Dictionary<string, Coflnet.Sky.Core.Services.Item>
            {
                ["TEST_ITEM"] = MakeItem("TEST_ITEM", color: "18:52:86", category: "HELMET") // 18:52:86 == 123456
            });
            var byId = new Dictionary<string, string> { { "ItemId", "999" } };

            // without the item context 123456 would count as exotic; resolved via ItemId it is the default
            Assert.That(MatchDb(filter, "Any", DbAuction(filter.FromHex("123456")), byId), Is.False, "item default color must not be exotic");
            Assert.That(MatchDb(filter, "Any", DbAuction(filter.FromHex("ABCDEF")), byId), Is.True, "a different unusual color is still exotic");
            // Original resolves to exactly the item default color
            Assert.That(MatchDb(filter, "Original", DbAuction(filter.FromHex("123456")), byId), Is.True, "default color is the original");
            Assert.That(MatchDb(filter, "Original", DbAuction(filter.FromHex("ABCDEF")), byId), Is.False);
        }

        [Test]
        public async Task DbExcludesDyedItems()
        {
            var filter = await CreateLoadedFilter();
            Assert.That(MatchDb(filter, "Any", DbAuction(filter.FromHex("123456"))), Is.True);
            Assert.That(MatchDb(filter, "Any", DbAuction(filter.FromHex("123456"), dyed: true)), Is.False, "dyed item should be excluded on db");
        }

        [Test]
        public async Task LegacyLabelWithColorListStillClassifies()
        {
            // filters stored before the clean-label change keep working: the label is used, colors ignored
            var filter = await CreateLoadedFilter();
            Assert.That(MatchInMemory(filter, "Any:123456,00FF00,ABCDEF", Auction("SUPERIOR_DRAGON_CHESTPLATE", "123456")), Is.True);
            Assert.That(MatchDb(filter, "Fairy:330066,FF00FF", DbAuction(filter.FromHex("FF00FF"))), Is.True);
        }

        private async Task<(ExoticColorFilter filter, HypixelItemService service, ItemDetails details)> CreateLoadedFilterWithItemData()
        {
            var provider = new ServiceCollection()
                .AddSingleton<HttpClient>()
                .AddSingleton(typeof(ILogger<>), typeof(NullLogger<>))
                .AddSingleton<HypixelItemService>()
                .AddSingleton<ExoticColorService>()
                // only TagLookup is used by the filter, the api client is never called here
                .AddSingleton(new ItemDetails(null))
                .BuildServiceProvider();
            var filter = new ExoticColorFilter();
            await filter.LoadData(provider);
            return (filter, provider.GetRequiredService<HypixelItemService>(), provider.GetRequiredService<ItemDetails>());
        }

        private static void SeedItems(HypixelItemService service, Dictionary<string, Coflnet.Sky.Core.Services.Item> items)
        {
            typeof(HypixelItemService)
                .GetField("_items", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(service, items);
        }

        private static Coflnet.Sky.Core.Services.Item MakeItem(string id, string color, string category)
            => new Coflnet.Sky.Core.Services.Item(null, color, null, category, null, 0, null, null, 0, null, false, id, null, null,
                null, null, null, null, null, null, null, null, null, null, null, null, null, null);
    }

    // maps color and dye keys to distinct ids so the db expressions can be exercised
    class KeyMapNbt : INBT
    {
        public const short ColorKey = 2;
        public const short DyeKey = 3;
        public short GetKeyId(string name) => name == "dye_item" ? DyeKey : ColorKey;
        public int GetValueId(short key, string value) => 0;
        public NBTLookup[] CreateLookup(string auctionTag, Dictionary<string, object> data, List<KeyValuePair<string, object>> flatList = null)
            => throw new System.NotImplementedException();
        public NBTLookup[] CreateLookup(SaveAuction auction) => throw new System.NotImplementedException();
        public long GetItemIdForSkin(string name) => throw new System.NotImplementedException();
    }
}
