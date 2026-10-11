using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Coflnet.Sky.Items.Client.Api;
using Coflnet.Sky.Items.Client.Model;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Linq;
using Coflnet.Sky.Core;
using NUnit.Framework;

namespace Coflnet.Sky.Filter.Tests;

public class RuneFilterTests
{
    private FilterEngine engine;

    [SetUp]
    public void Setup()
    {
        engine = new FilterEngine(new MockNbt());
    }

    private static SaveAuction WithRune(string key, string level)
        => new() { FlatenedNBT = new Dictionary<string, string> { { key, level } }, NBTLookup = new NBTLookup[0] };

    private List<SaveAuction> Select(string filterName, string value, params SaveAuction[] auctions)
        => engine.AddFilters(auctions.AsQueryable<IDbItem>(), new Dictionary<string, string> { { filterName, value } }, false).Cast<SaveAuction>().ToList();

    private static List<ItemPreview> Items(params string[] tags)
        => tags.Select(t => new ItemPreview { Tag = t, Name = t }).ToList();

    private void LoadFakeItems()
        => engine.UpdateRuneAliases(Items("RUNE", "UNIQUE_RUNE", "RUNE_SNOW", "RUNE_SOULTWIST", "UNIQUE_RUNE_GRAND_FREEZING", "RUNE_SACK", "RUNEBOOK", "SMALL_RUNES_SACK", "RUNE_", "UNIQUE_RUNE_"));

    private List<SaveAuction> Filtered(string filterName, string value, SaveAuction auction)
        => engine.Filter(new[] { auction }, new Dictionary<string, string> { { filterName, value } }).ToList();

    [TestCase("RUNE_SNOW", "RUNE_SNOW")]
    [TestCase("rune_snow", "RUNE_SNOW")]
    [TestCase("RUNE_SOULTWIST", "RUNE_SOULTWIST")]
    [TestCase("UNIQUE_RUNE_GRAND_FREEZING", "RUNE_GRAND_FREEZING")]
    [TestCase("RUNE_GRAND_FREEZING", "RUNE_GRAND_FREEZING")]
    [TestCase("rune_grand_freezing", "RUNE_GRAND_FREEZING")]
    public void KnownRuneSelectsThroughAddFiltersAndFilter(string filterName, string nbtKey)
    {
        LoadFakeItems();
        var withRune = WithRune(nbtKey, "3");
        // same engine, repeated and with other spellings in between
        foreach (var name in new[] { filterName, filterName.ToUpper(), filterName.ToLower(), filterName })
        {
            Assert.That(Select(name, "3", withRune), Is.EqualTo(new[] { withRune }), name);
            Assert.That(Select(name, "1", withRune), Is.Empty, name);
            Assert.That(Filtered(name, "3", withRune), Is.EqualTo(new[] { withRune }), name);
            Assert.That(Filtered(name, "1", withRune), Is.Empty, name);
        }
    }

    [TestCase("RUNE_MUSIC")]
    [TestCase("RUNE_ENCHANT")]
    [TestCase("RUNE_TIDAL")]
    [TestCase("RUNE_DRAGON")]
    public void SeededRunesWorkWithoutLoad(string nbtKey)
    {
        var withRune = WithRune(nbtKey, "3");
        Assert.That(Select(nbtKey, "3", withRune), Is.EqualTo(new[] { withRune }));
        Assert.That(Filtered(nbtKey.ToLower(), "3", withRune), Is.EqualTo(new[] { withRune }));
        Assert.That(Select(nbtKey, "1", withRune), Is.Empty);
    }

    [TestCase("RUNE_DOES_NOT_EXIST")]
    [TestCase("RUNE_")]
    [TestCase("UNIQUE_RUNE_")]
    [TestCase("RUNE_SACK")]
    [TestCase("RUNEBOOK")]
    [TestCase("DefinitelyNotAFilter")]
    public void UnknownKeysThrow(string name)
    {
        LoadFakeItems();
        var filters = new Dictionary<string, string> { { name, "1" } };
        Assert.Throws<UnknownFilterException>(() => engine.AddFilters(new SaveAuction[0].AsQueryable<IDbItem>(), filters, false));
        Assert.Throws<UnknownFilterException>(() => engine.Filter(new SaveAuction[0], filters));
    }

    [Test]
    public void EmptyUpdateKeepsPreviousRunes()
    {
        LoadFakeItems();
        engine.UpdateRuneAliases(new List<ItemPreview>());
        engine.UpdateRuneAliases(null);
        var withRune = WithRune("RUNE_SNOW", "3");
        Assert.That(Select("RUNE_SNOW", "3", withRune), Is.EqualTo(new[] { withRune }));
    }

    private static IServiceProvider ProviderWith(Func<Task<List<ItemPreview>>> names)
    {
        var api = new Mock<IItemsApi>();
        api.Setup(a => a.ItemNamesGetAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).Returns(names);
        return new ServiceCollection().AddSingleton(api.Object).BuildServiceProvider();
    }

    [Test]
    public async Task RefreshAddsNewRunesAndKeepsMapOnFailure()
    {
        var withNew = WithRune("RUNE_NEWCOMER", "3");
        AssertUnknownRune("RUNE_NEWCOMER");
        await engine.RefreshRuneAliases(ProviderWith(() => Task.FromResult(Items("RUNE_NEWCOMER"))));
        Assert.That(Select("RUNE_NEWCOMER", "3", withNew), Is.EqualTo(new[] { withNew }));
        await engine.RefreshRuneAliases(ProviderWith(() => throw new Exception("down")));
        await engine.RefreshRuneAliases(ProviderWith(() => Task.FromResult(Items())));
        await engine.RefreshRuneAliases(ProviderWith(() => Task.FromResult<List<ItemPreview>>(null)));
        Assert.That(Select("RUNE_NEWCOMER", "3", withNew), Is.EqualTo(new[] { withNew }));
        Assert.That(Select("RUNE_MUSIC", "3", WithRune("RUNE_MUSIC", "3")), Has.Count.EqualTo(1));
    }

    private void AssertUnknownRune(string name)
        => Assert.Throws<UnknownFilterException>(() => engine.Filter(new SaveAuction[0], new Dictionary<string, string> { { name, "1" } }));

    /// <summary>
    /// Saved filters go through GetMatcher/GetMatchExpression and behave like before, independent of the known runes.
    /// Pins HEAD behaviour of GetExpression (Core/FilterEngine.cs at HEAD, the Filters lookup, the "Rune" suffix fallback,
    /// the uppercase NecromancerFilter fallback and the UnknownFilterException) and GetFilter. Verified by running this test against HEAD's
    /// FilterEngine.cs with only no-op UpdateRuneAliases/RefreshRuneAliases stubs added so the test class compiles.
    /// </summary>
    [Test]
    public void MatcherKeepsOldBehaviour()
    {
        LoadFakeItems();
        foreach (var name in new[] { "RUNE_SNOW", "SnowRune" })
        {
            var matcher = engine.GetMatcher(new() { { name, "2" } });
            Assert.That(matcher(WithRune("RUNE_SNOW", "2")), Is.True, name);
            Assert.That(matcher(WithRune("RUNE_SNOW", "1")), Is.False, name);
        }
        // uppercase unknown key becomes a necromancer filter, mixed case unknown throws
        Assert.DoesNotThrow(() => engine.GetMatcher(new() { { "RUNE_DOES_NOT_EXIST_1", "2" } }));
        Assert.DoesNotThrow(() => engine.GetMatchExpression(new() { { "UPPER_UNKNOWN", "2" } }, true));
        Assert.Throws<UnknownFilterException>(() => engine.GetMatcher(new() { { "MixedCaseUnknown", "2" } }));
        Assert.Throws<UnknownFilterException>(() => engine.GetMatchExpression(new() { { "MixedCaseUnknown", "2" } }, true));
        Assert.Throws<CoflnetException>(() => engine.GetFilter("RUNE_SNOW"));
    }

    [Test]
    public void RunesAreNotAddedToAvailableFilters()
    {
        var before = engine.AvailableFilters.Select(f => f.Name).ToList();
        LoadFakeItems();
        Assert.That(engine.AvailableFilters.Select(f => f.Name).ToList(), Is.EqualTo(before));
        Assert.That(before, Does.Not.Contain("RUNE_SNOW"));
    }

    /// <summary>
    /// MusicRune is registered on HEAD (RUNE_MUSIC), without the TryParse in RuneFilter.ParseLevel
    /// the in-memory selector throws FormatException for these values
    /// </summary>
    [TestCase("abc")]
    [TestCase("NaN")]
    [TestCase("Infinity")]
    [TestCase("-Infinity")]
    [TestCase("1e300")]
    public void NonNumericNbtValueIsLevelZero(string value)
    {
        var bad = WithRune("RUNE_MUSIC", value);
        Assert.That(Filtered("MusicRune", "3", bad), Is.Empty);
        Assert.That(Filtered("MusicRune", "0", bad), Is.EqualTo(new[] { bad }));
        var badSecond = new SaveAuction { FlatenedNBT = new Dictionary<string, string> { { "MUSIC", value } }, NBTLookup = new NBTLookup[0] };
        Assert.That(Filtered("MusicRune", "3", badSecond), Is.Empty);
    }

    [Test]
    public void UnknownFilterStillThrows()
    {
        var filters = new Dictionary<string, string> { { "DefinitelyNotAFilter", "1" } };
        Assert.Throws<UnknownFilterException>(() => engine.AddFilters(new SaveAuction[0].AsQueryable<IDbItem>(), filters, false));
        Assert.Throws<UnknownFilterException>(() => engine.Filter(new SaveAuction[0], filters));
        Assert.Throws<UnknownFilterException>(() => engine.GetMatcher(filters));
        Assert.Throws<CoflnetException>(() => engine.GetFilter("DefinitelyNotAFilter"));
    }

    /// <summary>
    /// A rune whose key was never indexed gets key id 0 and has to select nothing instead of throwing
    /// </summary>
    [Test]
    public void NeverSeenRuneKeySelectsNothingInDb()
    {
        engine = new FilterEngine(new UnknownKeyNbt());
        engine.UpdateRuneAliases(Items("RUNE_BRAND_NEW"));
        var auction = new SaveAuction { NBTLookup = new[] { new NBTLookup(5, 3) } };
        var filters = new Dictionary<string, string> { { "RUNE_BRAND_NEW", "3" } };
        var result = engine.AddFilters(new[] { auction }.AsQueryable<SaveAuction>(), filters);
        Assert.That(result.ToList(), Is.Empty);
        Assert.That(engine.GetMatchExpression(filters, true).Compile()(auction), Is.False);
    }

    /// <summary>
    /// Like the real lookup: keys that were never indexed have id 0
    /// </summary>
    private class UnknownKeyNbt : INBT
    {
        public short GetKeyId(string name) => name == "known" ? (short)5 : (short)0;
        public int GetValueId(short key, string value) => 1;
        public NBTLookup[] CreateLookup(string auctionTag, Dictionary<string, object> data, List<KeyValuePair<string, object>> flatList = null) => throw new System.NotImplementedException();
        public NBTLookup[] CreateLookup(SaveAuction auction) => throw new System.NotImplementedException();
        public long GetItemIdForSkin(string name) => throw new System.NotImplementedException();
    }

    [Test]
    public void UniqueAndPlainTagTogetherBuild()
    {
        engine.UpdateRuneAliases(Items("UNIQUE_RUNE_GRAND_FREEZING", "RUNE_GRAND_FREEZING"));
        var withRune = WithRune("RUNE_GRAND_FREEZING", "3");
        Assert.That(Select("UNIQUE_RUNE_GRAND_FREEZING", "3", withRune), Is.EqualTo(new[] { withRune }));
        Assert.That(Select("RUNE_GRAND_FREEZING", "3", withRune), Is.EqualTo(new[] { withRune }));
    }

    private class LoaderFixture
    {
        public int Calls;
        public readonly TaskCompletionSource<bool> Reached = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public int SignalAt = int.MaxValue;
        public FilterLoader Loader;
        public FilterEngine Engine;

        public LoaderFixture(Func<int, List<ItemPreview>> result, TimeSpan refresh, TimeSpan retry)
        {
            Engine = new FilterEngine(new MockNbt());
            var api = new Mock<IItemsApi>();
            api.Setup(a => a.ItemNamesGetAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).Returns(() =>
            {
                var call = Interlocked.Increment(ref Calls);
                var list = result(call);
                if (call >= SignalAt)
                    Reached.TrySetResult(true);
                return Task.FromResult(list);
            });
            var provider = new ServiceCollection().AddSingleton(api.Object).BuildServiceProvider();
            Loader = new FilterLoader(Engine, provider, Microsoft.Extensions.Logging.Abstractions.NullLogger<FilterLoader>.Instance)
            { RefreshInterval = refresh, RetryInterval = retry };
        }

        public async Task WaitReached()
            => Assert.That(await Task.WhenAny(Reached.Task, Task.Delay(5000)), Is.SameAs(Reached.Task), "loader did not reach the expected call");
    }

    [Test]
    public async Task LoaderSurvivesFailuresAndAddsRuneLater()
    {
        // first call throws inside the api (swallowed, empty), second returns null, third works
        var f = new LoaderFixture(call => call switch { 1 => throw new Exception("down"), 2 => null, _ => Items("RUNE_LATER") },
            TimeSpan.FromMilliseconds(1), TimeSpan.FromMilliseconds(1));
        f.SignalAt = 3;
        using var cts = new CancellationTokenSource();
        var run = f.Loader.RefreshLoop(cts.Token);
        try { await f.WaitReached(); }
        finally { cts.Cancel(); await run; }
        Assert.That(f.Engine.RunesLoaded, Is.True);
        var auction = WithRune("RUNE_LATER", "3");
        Assert.That(f.Engine.Filter(new[] { auction }, new Dictionary<string, string> { { "RUNE_LATER", "3" } }).ToList(), Is.EqualTo(new[] { auction }));
    }

    [Test]
    public async Task LoaderStopsOnCancellation()
    {
        var f = new LoaderFixture(_ => Items("RUNE_X"), TimeSpan.FromHours(1), TimeSpan.FromHours(1));
        using var cts = new CancellationTokenSource();
        var run = f.Loader.RefreshLoop(cts.Token);
        try
        {
            await Task.Delay(50); // loop is waiting in its 1 h delay
            Assert.That(run.IsCompleted, Is.False);
            cts.Cancel();
            Assert.That(await Task.WhenAny(run, Task.Delay(5000)), Is.SameAs(run));
            Assert.That(f.Calls, Is.EqualTo(0));
        }
        finally { cts.Cancel(); await run; }
    }

    [Test]
    public async Task HangingRefreshDoesNotBlockShutdownOrReplaceMap()
    {
        var api = new Mock<IItemsApi>();
        var started = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        api.Setup(a => a.ItemNamesGetAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns(() => { started.TrySetResult(true); return new TaskCompletionSource<List<ItemPreview>>().Task; });
        var provider = new ServiceCollection().AddSingleton(api.Object).BuildServiceProvider();
        var loader = new FilterLoader(engine, provider, Microsoft.Extensions.Logging.Abstractions.NullLogger<FilterLoader>.Instance)
        { RefreshInterval = TimeSpan.FromMilliseconds(1), RetryInterval = TimeSpan.FromMilliseconds(1) };
        using var cts = new CancellationTokenSource();
        var run = loader.RefreshLoop(cts.Token);
        try
        {
            Assert.That(await Task.WhenAny(started.Task, Task.Delay(5000)), Is.SameAs(started.Task));
            cts.Cancel();
            Assert.That(await Task.WhenAny(run, Task.Delay(5000)), Is.SameAs(run));
            Assert.That(engine.RunesLoaded, Is.False);
        }
        finally { cts.Cancel(); await run; }
    }

    [Test]
    public async Task CancelledRefreshThrowsInsteadOfFailing()
    {
        var api = new Mock<IItemsApi>();
        api.Setup(a => a.ItemNamesGetAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).Returns<int, CancellationToken>((_, t) => Task.FromCanceled<List<ItemPreview>>(t));
        var provider = new ServiceCollection().AddSingleton(api.Object).BuildServiceProvider();
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        Assert.CatchAsync<OperationCanceledException>(async () => await engine.RefreshRuneAliases(provider, cts.Token));
        await Task.CompletedTask;
    }

    [Test]
    public void NullElementInListDoesNotThrow()
    {
        Assert.That(engine.UpdateRuneAliases(new List<ItemPreview> { null, new ItemPreview { Tag = "RUNE_SNOW" } }), Is.True);
        Assert.That(engine.UpdateRuneAliases(new List<ItemPreview> { null }), Is.False);
    }

    [Test]
    public async Task SeedOnlyMapCallsWithSameIntervals()
    {
        // positive counterpart of LoadedMapUsesLongInterval: same intervals, but nothing loaded yet
        var f = new LoaderFixture(_ => new List<ItemPreview>(), TimeSpan.FromHours(1), TimeSpan.FromMilliseconds(1));
        f.SignalAt = 2;
        using var cts = new CancellationTokenSource();
        var run = f.Loader.RefreshLoop(cts.Token);
        try { await f.WaitReached(); }
        finally { cts.Cancel(); await run; }
    }

    [Test]
    public async Task SeedOnlyMapRetriesWithShortInterval()
    {
        // the long interval would never fire within the test, so reaching call 3 proves the retry interval is used
        var f = new LoaderFixture(call => call < 3 ? new List<ItemPreview>() : Items("RUNE_RETRY"),
            TimeSpan.FromHours(1), TimeSpan.FromMilliseconds(1));
        f.SignalAt = 3;
        using var cts = new CancellationTokenSource();
        var run = f.Loader.RefreshLoop(cts.Token);
        try { await f.WaitReached(); }
        finally { cts.Cancel(); await run; }
        Assert.That(f.Engine.RunesLoaded, Is.True);
    }

    [Test]
    public async Task LoadedMapUsesLongInterval()
    {
        var f = new LoaderFixture(_ => Items("RUNE_X"), TimeSpan.FromHours(1), TimeSpan.FromMilliseconds(1));
        f.Engine.UpdateRuneAliases(Items("RUNE_X"));
        using var cts = new CancellationTokenSource();
        var run = f.Loader.RefreshLoop(cts.Token);
        try
        {
            await Task.Delay(50);
            Assert.That(f.Calls, Is.EqualTo(0));
        }
        finally { cts.Cancel(); await run; }
    }

    [Test]
    public void ListWithoutRunesKeepsPreviousMap()
    {
        LoadFakeItems();
        Assert.That(engine.UpdateRuneAliases(Items("SOMETHING", "RUNEBOOK", "RUNE_SACK", "RUNE_")), Is.False);
        var withRune = WithRune("RUNE_SNOW", "3");
        Assert.That(Select("RUNE_SNOW", "3", withRune), Is.EqualTo(new[] { withRune }));
        Assert.That(engine.UpdateRuneAliases(Items("RUNE_SNOW")), Is.True);
    }
}
