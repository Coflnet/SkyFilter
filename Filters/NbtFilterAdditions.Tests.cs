using System;
using System.Collections.Generic;
using System.Linq;
using Coflnet.Sky.Core;
using Item = Coflnet.Sky.Items.Client.Model.Item;
using Modifiers = Coflnet.Sky.Items.Client.Model.Modifiers;
using NUnit.Framework;

namespace Coflnet.Sky.Filter;

public class NbtFilterAdditionsTests
{
    private readonly FilterEngine engine = new(new KeyedNbt());

    private static Item ItemWith(string tag, params string[] modifierSlugs)
        => new(tag: tag, modifiers: modifierSlugs.Select(s => new Modifiers(slug: s, value: "")).ToList());

    private bool Matches(string filterName, string value, bool targetsDb, SaveAuction auction)
    {
        var filters = new Dictionary<string, string> { { filterName, value } };
        return engine.GetMatchExpression(filters, targetsDb).Compile()(auction);
    }

    private static SaveAuction FlatAuction(string key, string value, string tag = "HELIX_TEST")
        => new() { Tag = tag, FlatenedNBT = new() { { key, value } }, NBTLookup = [] };

    private static SaveAuction LookupAuction(string key, long value, string tag = "HELIX_TEST")
        => new() { Tag = tag, NBTLookup = [new NBTLookup(KeyedNbt.Id(key), value)], FlatenedNBT = new() { { key, value.ToString() } } };

    [TestCase("HelixForagingFortune", "foraging_fortune")]
    [TestCase("HelixForagingWisdom", "foraging_wisdom")]
    [TestCase("HelixSweep", "sweep")]
    [TestCase("HelixHuntingWisdom", "hunting_wisdom")]
    [TestCase("HelixFighting", "fighting")]
    public void HelixBoosterMatchesTier(string filterName, string key)
    {
        Assert.That(Matches(filterName, "2", false, FlatAuction(key, "2")));
        Assert.That(Matches(filterName, "2", true, LookupAuction(key, 2)));
        Assert.That(Matches(filterName, "1", true, LookupAuction(key, 2)), Is.False);
        Assert.That(Matches(filterName, "1-2", true, LookupAuction(key, 2)));
    }

    [TestCase("HelixSweep")]
    [TestCase("HelixFighting")]
    public void HelixBoosterOnlyApplicableToHelixItems(string filterName)
    {
        var filter = engine.AvailableFilters.First(f => f.Name == filterName);
        var key = filterName == "HelixSweep" ? "sweep" : "fighting";
        Assert.That(filter.IsApplicable(ItemWith("HELIX_CHOPPER", key)));
        Assert.That(filter.IsApplicable(ItemWith("HELIX_LOG")), Is.False, "helix item without the booster");
        Assert.That(filter.IsApplicable(ItemWith("TERMINATOR", "sweep", "fighting")), Is.False);
    }

    [TestCase("HelixSweep", "sweep")]
    [TestCase("HelixFighting", "fighting")]
    public void HelixBoosterDoesNotMatchNonHelixItems(string filterName, string key)
    {
        Assert.That(Matches(filterName, "2", false, FlatAuction(key, "2", "TERMINATOR")), Is.False);
        Assert.That(Matches(filterName, "2", true, LookupAuction(key, 2, "TERMINATOR")), Is.False);
        Assert.That(Matches(filterName, "2", false, FlatAuction(key, "2", "HELIX_CHOPPER")));
        Assert.That(Matches(filterName, "2", true, LookupAuction(key, 2, "HELIX_CHOPPER")));
    }

    [TestCase("0")]
    [TestCase("None")]
    [TestCase("1-2")]
    public void HelixBoosterIgnoresAuctionWithoutTag(string query)
    {
        var withBooster = new SaveAuction { Tag = null, FlatenedNBT = new() { { "sweep", "2" } }, NBTLookup = [] };
        var withoutBooster = new SaveAuction { Tag = null, FlatenedNBT = new(), NBTLookup = [] };

        Assert.That(Matches("HelixSweep", query, false, withBooster), Is.False);
        Assert.That(Matches("HelixSweep", query, false, withoutBooster), Is.False);
    }

    [Test]
    public void PreviouslyExistingFilterNamesStillRegistered()
    {
        var names = engine.AvailableFilters.Select(f => f.Name).ToList();
        Assert.That(names, Is.Unique.IgnoreCase);
        string[] required = ["mining0Gem", "mining1Gem", "mining0GemType", "mining1GemType", "AxeBoosters", "GrowthStages",
            "ruby0Gem", "combat0Gem", "universal1Gem", "chisel0Gem", "chisel1GemType", "Reforge", "Rarity"];
        Assert.That(names, Is.SupersetOf(required).IgnoreCase);
    }

    [Test]
    public void MiningGemNoneNoLongerMatchesSetSlot()
    {
        var auction = FlatAuction("MINING_0", "PERFECT", "DIVAN_HELMET");
        Assert.That(Matches("mining0Gem", "None", false, auction), Is.False);
        Assert.That(Matches("mining0Gem", "Any", false, auction));
        Assert.That(Matches("mining0Gem", "None", false, FlatAuction("other", "1", "DIVAN_HELMET")));
    }

    [TestCase("MINING_0", "mining0Gem", "mining0GemType")]
    [TestCase("MINING_1", "mining1Gem", "mining1GemType")]
    public void MiningGemSlotMatchesRealKey(string key, string gemFilter, string typeFilter)
    {
        Assert.That(Matches(gemFilter, "PERFECT", false, FlatAuction(key, "PERFECT")));
        Assert.That(Matches(gemFilter, "FLAWLESS", false, FlatAuction(key, "PERFECT")), Is.False);
        Assert.That(Matches(typeFilter, "TOPAZ", false, FlatAuction(key + "_gem", "TOPAZ")));
    }

    [Test]
    public void MiningGemSlotMatchesInDb()
    {
        var auction = new SaveAuction()
        {
            NBTLookup = [new NBTLookup(KeyedNbt.Id("MINING_0"), KeyedNbt.ValueId("PERFECT")),
                new NBTLookup(KeyedNbt.Id("MINING_0_gem"), KeyedNbt.ValueId("TOPAZ"))]
        };
        Assert.That(Matches("mining0Gem", "PERFECT", true, auction));
        Assert.That(Matches("mining0GemType", "TOPAZ", true, auction));
    }

    [TestCase("RUBY_2", "ruby2Gem")]
    [TestCase("RUBY_3", "ruby3Gem")]
    [TestCase("RUBY_4", "ruby4Gem")]
    [TestCase("CHISEL_2", "chisel2Gem")]
    public void ExtraGemSlotsMatch(string key, string filterName)
    {
        Assert.That(Matches(filterName, "FLAWLESS", false, FlatAuction(key, "FLAWLESS")));
        Assert.That(Matches(filterName, "PERFECT", false, FlatAuction(key, "FLAWLESS")), Is.False);
        var filter = engine.AvailableFilters.First(f => f.Name == filterName);
        Assert.That(filter.IsApplicable(ItemWith("MASTIFF_CHESTPLATE", key)));
        Assert.That(filter.IsApplicable(ItemWith("OTHER")), Is.False);
    }

    [Test]
    public void ChiselThirdSlotTypeMatches()
    {
        Assert.That(Matches("chisel2GemType", "JASPER", false, FlatAuction("CHISEL_2_gem", "JASPER")));
    }

    [TestCase("SecondsHeld", "seconds_held", "380000", ">=1")]
    [TestCase("StatsBook", "stats_book", "1500", "1000-2000")]
    [TestCase("BossTier", "boss_tier", "4", "4")]
    [TestCase("ChampionCombatXp", "champion_combat_xp", "250000", ">100000")]
    [TestCase("HecatombRuns", "hecatomb_s_runs", "50", ">=50")]
    [TestCase("CompactBlocks", "compact_blocks", "9000", "5000-10000")]
    [TestCase("ToxophiliteCombatXp", "toxophilite_combat_xp", "12", "<20")]
    [TestCase("PotionLevel", "potion_level", "5", "5")]
    public void CounterFilterMatchesAndApplicability(string filterName, string key, string stored, string query)
    {
        Assert.That(Matches(filterName, query, false, FlatAuction(key, stored)));
        Assert.That(Matches(filterName, query, true, LookupAuction(key, long.Parse(stored))));
        Assert.That(Matches(filterName, "99999999", true, LookupAuction(key, long.Parse(stored))), Is.False);

        var filter = engine.AvailableFilters.First(f => f.Name == filterName);
        Assert.That(filter.IsApplicable(ItemWith("ANY", key)));
        Assert.That(filter.IsApplicable(ItemWith("ANY", "unrelated")), Is.False);
    }

    [Test]
    public void PotionTypeMatchesAndIsApplicable()
    {
        Assert.That(Matches("Potion", "harvest_harbinger", false, FlatAuction("potion", "harvest_harbinger")));
        Assert.That(Matches("Potion", "stinky_cheese", false, FlatAuction("potion", "harvest_harbinger")), Is.False);
        var filter = engine.AvailableFilters.First(f => f.Name == "Potion");
        Assert.That(filter.IsApplicable(ItemWith("POTION", "potion", "potion_level")));
        Assert.That(filter.IsApplicable(ItemWith("TERMINATOR")), Is.False);
    }

    [Test]
    public void GrowthStagesStillRosewaterSpecific()
    {
        var filter = engine.AvailableFilters.First(f => f.Name == "GrowthStages");
        Assert.That(filter.IsApplicable(ItemWith("ROSEWATER_FLASK")));
        Assert.That(filter.IsApplicable(ItemWith("MOBY_DUCK", "seconds_held")), Is.False);
    }

    [Test]
    public void SecondsHeldCoversOtherHeldItems()
    {
        var filter = engine.AvailableFilters.First(f => f.Name == "SecondsHeld");
        foreach (var tag in new[] { "MOBY_DUCK", "DARK_CACAO_TRUFFLE", "NOT_VERY_MOLDY_BREAD" })
            Assert.That(filter.IsApplicable(ItemWith(tag, "seconds_held")), tag);
    }

    /// <summary>
    /// Deterministic key and value ids so db style lookups can be built by tests
    /// </summary>
    private class KeyedNbt : INBT
    {
        private static readonly Dictionary<string, short> Keys = new();
        private static readonly Dictionary<string, int> Values = new();

        public static short Id(string key)
        {
            lock (Keys)
                return Keys.TryGetValue(key, out var id) ? id : Keys[key] = (short)(Keys.Count + 1);
        }

        public static int ValueId(string value)
        {
            lock (Values)
                return Values.TryGetValue(value, out var id) ? id : Values[value] = Values.Count + 1;
        }

        public short GetKeyId(string name) => Id(name);
        public int GetValueId(short key, string value) => ValueId(value);
        public NBTLookup[] CreateLookup(string auctionTag, Dictionary<string, object> data, List<KeyValuePair<string, object>> flatList = null)
            => throw new NotImplementedException();
        public NBTLookup[] CreateLookup(SaveAuction auction) => throw new NotImplementedException();
        public long GetItemIdForSkin(string name) => throw new NotImplementedException();
    }
}
