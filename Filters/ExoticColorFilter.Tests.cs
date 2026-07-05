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
            var value = $"Fairy:{string.Join(',', ExoticColorFilter.FairyColors)}";
            var exp = filter.GetExpression(new FilterArgs(new Dictionary<string, string>() { { "ExoticColor", value } }, false, filterEngine));
            var match = exp.Compile();
            Assert.That(match(Auction("SUPERIOR_DRAGON_CHESTPLATE", "FF00FF")), Is.True, "fairy color should match");
            Assert.That(match(Auction("SUPERIOR_DRAGON_CHESTPLATE", "123456")), Is.False, "random color should not match fairy");
        }

        [Test]
        public async Task CrystalColorMatchesViaService()
        {
            var filter = await CreateLoadedFilter();
            var value = $"Crystal:{string.Join(',', ExoticColorFilter.CrystalColors)}";
            var exp = filter.GetExpression(new FilterArgs(new Dictionary<string, string>() { { "ExoticColor", value } }, false, filterEngine));
            var match = exp.Compile();
            Assert.That(match(Auction("SUPERIOR_DRAGON_CHESTPLATE", "1F0030")), Is.True, "crystal color should match");
            Assert.That(match(Auction("SUPERIOR_DRAGON_CHESTPLATE", "FF00FF")), Is.False, "fairy color should not match crystal");
        }

        [Test]
        public async Task AnyMatchesExoticButNotFairyOrCrystal()
        {
            var filter = await CreateLoadedFilter();
            var exp = filter.GetExpression(new FilterArgs(new Dictionary<string, string>() { { "ExoticColor", "Any:123456" } }, false, filterEngine));
            var match = exp.Compile();
            Assert.That(match(Auction("SUPERIOR_DRAGON_CHESTPLATE", "123456")), Is.True, "unusual color should count as exotic");
            Assert.That(match(Auction("SUPERIOR_DRAGON_CHESTPLATE", "FF00FF")), Is.False, "fairy color is not plain exotic");
            Assert.That(match(Auction("SUPERIOR_DRAGON_CHESTPLATE", "1F0030")), Is.False, "crystal color is not plain exotic");
        }

        [Test]
        public async Task ExoticMatchesOnlyPlainExotic()
        {
            var filter = await CreateLoadedFilter();
            var exp = filter.GetExpression(new FilterArgs(new Dictionary<string, string>() { { "ExoticColor", "Exotic:123456" } }, false, filterEngine));
            var match = exp.Compile();
            Assert.That(match(Auction("SUPERIOR_DRAGON_CHESTPLATE", "123456")), Is.True, "unusual color is exotic");
            Assert.That(match(Auction("FAIRY_HELMET", "000000")), Is.False, "spook color is not plain exotic");
            Assert.That(match(Auction("SUPERIOR_DRAGON_CHESTPLATE", "FF00FF")), Is.False, "fairy color is not plain exotic");
        }

        [Test]
        public async Task PerColorTypeOptionsClassifyViaService()
        {
            var filter = await CreateLoadedFilter();

            static bool Match(ExoticColorFilter f, FilterEngine e, string value, SaveAuction auction)
                => f.GetExpression(new FilterArgs(new Dictionary<string, string>() { { "ExoticColor", value } }, false, e)).Compile()(auction);

            // Undyed
            Assert.That(Match(filter, filterEngine, "Undyed:A06540", Auction("SUPERIOR_DRAGON_CHESTPLATE", "A06540")), Is.True);
            Assert.That(Match(filter, filterEngine, "Undyed:A06540", Auction("SUPERIOR_DRAGON_CHESTPLATE", "123456")), Is.False);
            // Spook (fairy item wearing a spook color)
            Assert.That(Match(filter, filterEngine, "Spook:", Auction("FAIRY_HELMET", "000000")), Is.True);
            Assert.That(Match(filter, filterEngine, "Spook:", Auction("FAIRY_HELMET", "123456")), Is.False);
            // Glitched (other-glitched mapping)
            Assert.That(Match(filter, filterEngine, "Glitched:", Auction("SHARK_SCALE_CHESTPLATE", "FFDC51")), Is.True);
            Assert.That(Match(filter, filterEngine, "Glitched:", Auction("SHARK_SCALE_CHESTPLATE", "123456")), Is.False);
            // Plain Fairy (fairy color that is not an OG fairy color)
            Assert.That(Match(filter, filterEngine, "Plain Fairy:", Auction("SUPERIOR_DRAGON_CHESTPLATE", "FF007F")), Is.True);
            Assert.That(Match(filter, filterEngine, "Plain Fairy:", Auction("SUPERIOR_DRAGON_CHESTPLATE", "123456")), Is.False);
        }

        [Test]
        public void OptionsIncludeEveryClassifiableColorType()
        {
            var filter = new ExoticColorFilter();
            var options = filter.OptionsGet(new OptionValues(new Dictionary<string, List<string>>
            {
                ["color"] = new() { "0", "16711680", "255" }
            })).Select(o => o.ToString().Split(':')[0]).ToHashSet();

            // every ColorType the service can return must be reachable through some option
            Assert.That(options, Does.Contain("Any"));
            Assert.That(options, Does.Contain("Exotic"));
            Assert.That(options, Does.Contain("Crystal"));
            Assert.That(options, Does.Contain("Fairy"));
            Assert.That(options, Does.Contain("Plain Fairy"));
            Assert.That(options, Does.Contain("OG Fairy"));
            Assert.That(options, Does.Contain("Undyed"));
            Assert.That(options, Does.Contain("Original"));
            Assert.That(options, Does.Contain("Glitched"));
            Assert.That(options, Does.Contain("Spook"));
        }

        [Test]
        public async Task ExplicitHexStillMatchesExactColor()
        {
            var filter = await CreateLoadedFilter();
            // A raw hex value (no category label) keeps the plain color-value match.
            var exp = filter.GetExpression(new FilterArgs(new Dictionary<string, string>() { { "ExoticColor", "ABCDEF" } }, false, filterEngine));
            var match = exp.Compile();
            Assert.That(match(Auction("SUPERIOR_DRAGON_CHESTPLATE", "ABCDEF")), Is.True);
            Assert.That(match(Auction("SUPERIOR_DRAGON_CHESTPLATE", "123456")), Is.False);
        }
    }
}
