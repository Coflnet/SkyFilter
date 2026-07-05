using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Coflnet.Sky.Core;
using Coflnet.Sky.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using ColorType = Coflnet.Sky.Core.Services.ExoticColorService.ExoticColorType;

namespace Coflnet.Sky.Filter;
[FilterDescription("Exotic colors, Any is all kinds of exotics including fairy and crystal. Special exotics are just exotic.")]
public class ExoticColorFilter : ColorFilter
{
    // Sourced from the shared classification service so the filter stays in sync with the server/mod.
    public static IEnumerable<string> FairyColors => Coflnet.Sky.Core.Services.FairyColors.fairyColourConstants;
    public static IEnumerable<string> CrystalColors => ExoticColorService.crystalColours;

    /// <summary>
    /// Maps the human readable option label to the set of <see cref="ColorType"/> that should match.
    /// Only used for the in memory path where the <see cref="ExoticColorService"/> is available.
    /// </summary>
    private static readonly Dictionary<string, HashSet<ColorType>> TypesByLabel = new(StringComparer.OrdinalIgnoreCase)
    {
        // combos
        ["Any"] = new() { ColorType.EXOTIC, ColorType.GLITCHED, ColorType.SPOOK },
        ["Fairy"] = new() { ColorType.FAIRY, ColorType.OG_FAIRY },
        ["Fairy+Crystal"] = new() { ColorType.FAIRY, ColorType.OG_FAIRY, ColorType.CRYSTAL },
        // one entry per ColorType the service can classify (DEFAULT is never returned - it maps to ORIGINAL)
        ["Crystal"] = new() { ColorType.CRYSTAL },
        ["Plain Fairy"] = new() { ColorType.FAIRY },
        ["OG Fairy"] = new() { ColorType.OG_FAIRY },
        ["Undyed"] = new() { ColorType.UNDYED },
        ["Original"] = new() { ColorType.ORIGINAL },
        ["Exotic"] = new() { ColorType.EXOTIC },
        ["Glitched"] = new() { ColorType.GLITCHED },
        ["Spook"] = new() { ColorType.SPOOK },
    };

    private ExoticColorService exoticColorService;

    public override Task LoadData(IServiceProvider provider)
    {
        // Optional: only present where the shared item state is synced (e.g. SkyBFCS / mod backend).
        // When absent we transparently fall back to the color-value match below.
        exoticColorService = provider.GetService<ExoticColorService>();
        return base.LoadData(provider);
    }

    public override IEnumerable<object> OptionsGet(OptionValues options)
    {
        var fairy = FairyColors.ToHashSet();
        var crystal = CrystalColors.ToHashSet();
        var all = options.Options["color"]
            // the most common one is the default
            .Skip(1)
            .Select(dec => ToHex(dec))
            .Where(hex =>
                !fairy.Contains(hex) && !crystal.Contains(hex)
                && hex != "A06540" // "bleached" color (normal brown) no special value
                )
            // only the rarest 10 are of interest
            .Reverse().Take(200)
            .ToList();

        if (all.Count > 0)
        {
            // both carry the rarest colors for the database fallback; the in memory path
            // narrows "Exotic" down to just the EXOTIC classification (no glitched/spook).
            var joined = string.Join(',', all);
            all.Insert(0, $"Exotic:{joined}");
            all.Insert(0, $"Any:{joined}");
        }
        var ogFairy = Coflnet.Sky.Core.Services.FairyColors.ogFairyColourConstants;
        return all
            // combos
            .Append($"Fairy:{string.Join(',', fairy)}")
            .Append($"Fairy+Crystal:{string.Join(',', fairy.Concat(crystal))}")
            // one option per ColorType. The color list is only used for the database / explicit
            // fallback; the in memory matcher classifies via the service by the label prefix, so
            // types without a fixed color set (Original/Glitched/Spook) carry an empty list there.
            .Append($"Crystal:{string.Join(',', crystal)}")
            .Append($"Plain Fairy:{string.Join(',', fairy)}")
            .Append($"OG Fairy:{string.Join(',', ogFairy)}")
            .Append("Undyed:A06540")
            .Append("Original:")
            .Append("Glitched:")
            .Append("Spook:");
    }

    public override Expression<Func<IDbItem, bool>> GetExpression(FilterArgs args)
    {
        var stringVal = args.Get(this);
        var label = stringVal.Contains(':') ? stringVal.Split(':')[0] : null;
        // Category labels (Any/Fairy/Crystal/...) are classified per item via the shared service.
        // This is the path used by the in memory matcher (SkyBFCS state sync); it needs the item id
        // and creation time that a plain color set can not express. Explicit hex values and the
        // database path keep using the color-value match for backwards compatibility.
        if (!args.TargetsDB && exoticColorService != null
            && label != null && TypesByLabel.TryGetValue(label, out var types))
        {
            return a => MatchesType(a as SaveAuction, types);
        }
        var values = stringVal.Split(':').Last().Split(',')
            .Where(v => !string.IsNullOrWhiteSpace(v)).Select(hex => FromHex(hex)).ToHashSet();
        if (!args.TargetsDB)
            return a => (a as SaveAuction).FlatenedNBT.Where(n => n.Key == PropName).Select(n => NBT.GetColor(n.Value)).Any(c => values.Contains(c));
        var key = args.NbtIntance.GetKeyId("color");

        return a => a.NBTLookup.Where(l => l.KeyId == key && values.Contains(l.Value)).Any();
    }

    private bool MatchesType(SaveAuction auction, HashSet<ColorType> types)
    {
        if (auction?.FlatenedNBT == null || auction.Tag == null)
            return false;
        if (!auction.FlatenedNBT.TryGetValue(PropName, out var colorVal) || colorVal == null)
            return false;
        // GetExoticColorType needs a plain RRGGBB hex and a millisecond creation timestamp.
        var creation = new DateTimeOffset(DateTime.SpecifyKind(auction.ItemCreatedAt, DateTimeKind.Utc)).ToUnixTimeMilliseconds();
        var type = exoticColorService.GetExoticColorType(auction.Tag, ToHex(colorVal), creation);
        return types.Contains(type);
    }
}

public class FairyColorFilter : ExoticColorFilter
{
    public override IEnumerable<object> OptionsGet(OptionValues options)
    {
        return new string[] { $"Fairy:{string.Join(',', FairyColors)}" };
    }
}
public class CrystalColorFilter : ExoticColorFilter
{
    public override IEnumerable<object> OptionsGet(OptionValues options)
    {
        return new string[] { $"Crystal:{string.Join(',', CrystalColors)}" };
    }
}
