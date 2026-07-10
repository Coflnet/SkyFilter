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

    // Fixed, item independent color sets. Used to rebuild a database query (and the service-less in
    // memory fallback) from constants, so these colors never have to travel inside the filter value.
    private static IEnumerable<string> FairyHexes => Coflnet.Sky.Core.Services.FairyColors.fairyColourConstants;
    private static IEnumerable<string> OgFairyHexes => Coflnet.Sky.Core.Services.FairyColors.ogFairyColourConstants;
    private static IEnumerable<string> CrystalHexes => ExoticColorService.crystalColours;
    private static IEnumerable<string> SpookHexes => ExoticColorService.spookColours;
    private static IEnumerable<string> GlitchedHexes => GlitchedColours.OTHER_GLITCHED.Keys
        .Concat(GlitchedColours.CHESTPLATE_COLOURS.Keys)
        .Concat(GlitchedColours.LEGGINGS_COLOURS.Keys)
        .Concat(GlitchedColours.BOOT_COLOURS.Keys);
    private const string UndyedHex = "A06540";

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
    private HypixelItemService itemService;
    private ItemDetails itemDetails;

    public override Task LoadData(IServiceProvider provider)
    {
        // Optional: only present where the shared item state is synced (e.g. SkyBFCS / mod backend).
        // When absent we transparently fall back to the color-value match below.
        exoticColorService = provider.GetService<ExoticColorService>();
        // Used on the database path to resolve the item default color from the ItemId in the query.
        itemService = provider.GetService<HypixelItemService>();
        itemDetails = provider.GetService<ItemDetails>();
        return base.LoadData(provider);
    }

    public override IEnumerable<object> OptionsGet(OptionValues options)
    {
        // Only offer exotic color options for items that actually have colors.
        if (!options.Options.TryGetValue("color", out var colors) || colors.Count == 0)
            return Enumerable.Empty<object>();

        // Plain labels only. The in memory path classifies each item via the service; the database
        // path rebuilds the color set from constants and resolves the item default color from the
        // ItemId present in the query (see GetItemDefaultColor).
        return new object[]
        {
            "Any",
            "Exotic",
            "Original",
            "Fairy",
            "Fairy+Crystal",
            "Crystal",
            "Plain Fairy",
            "OG Fairy",
            "Undyed",
            "Glitched",
            "Spook",
        };
    }

    public override Expression<Func<IDbItem, bool>> GetExpression(FilterArgs args)
    {
        var stringVal = args.Get(this);
        // Older stored filters may still carry "Label:colors"; keep the label, ignore the colors.
        var separator = stringVal.IndexOf(':');
        var label = separator >= 0 ? stringVal.Substring(0, separator) : stringVal;

        // Most accurate: classify each item per its id and creation time via the shared service.
        // This is the in memory path used by the state synced matcher (SkyBFCS / mod / flips).
        if (!args.TargetsDB && exoticColorService != null && TypesByLabel.ContainsKey(label))
            return a => MatchesType(a as SaveAuction, TypesByLabel[label]);

        // Rebuild the color set from constants (+ the item default) so the database query and the
        // service-less in memory fallback work without the value carrying a color list.
        if (TypesByLabel.ContainsKey(label))
            return ColorSetExpression(args, label);

        // Explicit hex value(s): plain color match, backwards compatible with stored filters.
        return ExplicitHexExpression(args, stringVal);
    }

    private Expression<Func<IDbItem, bool>> ColorSetExpression(FilterArgs args, string label)
    {
        // "Any"/"Exotic" match anything that is not the item default nor a known normal color.
        if (Is(label, "Any") || Is(label, "Exotic"))
        {
            var normal = ToValues(FairyHexes.Concat(OgFairyHexes).Concat(CrystalHexes).Append(UndyedHex));
            var itemDefault = GetItemDefaultColor(args);
            if (itemDefault.HasValue)
                normal.Add(itemDefault.Value);
            return ExcludeExpression(args, normal);
        }

        HashSet<long> include;
        if (Is(label, "Original"))
        {
            var itemDefault = GetItemDefaultColor(args);
            include = itemDefault.HasValue ? new HashSet<long> { itemDefault.Value } : new HashSet<long>();
        }
        else if (Is(label, "Fairy"))
            include = ToValues(FairyHexes.Concat(OgFairyHexes));
        else if (Is(label, "Plain Fairy"))
            include = ToValues(FairyHexes);
        else if (Is(label, "OG Fairy"))
            include = ToValues(OgFairyHexes);
        else if (Is(label, "Fairy+Crystal"))
            include = ToValues(FairyHexes.Concat(OgFairyHexes).Concat(CrystalHexes));
        else if (Is(label, "Crystal"))
            include = ToValues(CrystalHexes);
        else if (Is(label, "Undyed"))
            include = ToValues(new[] { UndyedHex });
        else if (Is(label, "Glitched"))
            include = ToValues(GlitchedHexes);
        else if (Is(label, "Spook"))
            include = ToValues(SpookHexes);
        else
            include = new HashSet<long>();
        return IncludeExpression(args, include);
    }

    // matches items whose color is in the given set (and that are not dyed)
    private Expression<Func<IDbItem, bool>> IncludeExpression(FilterArgs args, HashSet<long> colors)
    {
        if (!args.TargetsDB)
            return a => !(a as SaveAuction).FlatenedNBT.ContainsKey(DyeKey)
                && (a as SaveAuction).FlatenedNBT.Where(n => n.Key == PropName)
                    .Select(n => NBT.GetColor(n.Value)).Any(c => colors.Contains(c));
        var key = args.NbtIntance.GetKeyId(PropName);
        var dyeKey = args.NbtIntance.GetKeyId(DyeKey);
        return a => a.NBTLookup.Any(l => l.KeyId == key && colors.Contains(l.Value))
            && !a.NBTLookup.Any(l => l.KeyId == dyeKey);
    }

    // matches items whose color is NOT in the given "normal" set (and that are not dyed)
    private Expression<Func<IDbItem, bool>> ExcludeExpression(FilterArgs args, HashSet<long> normalColors)
    {
        if (!args.TargetsDB)
            return a => !(a as SaveAuction).FlatenedNBT.ContainsKey(DyeKey)
                && (a as SaveAuction).FlatenedNBT.Where(n => n.Key == PropName)
                    .Select(n => NBT.GetColor(n.Value)).Any(c => !normalColors.Contains(c));
        var key = args.NbtIntance.GetKeyId(PropName);
        var dyeKey = args.NbtIntance.GetKeyId(DyeKey);
        return a => a.NBTLookup.Any(l => l.KeyId == key && !normalColors.Contains(l.Value))
            && !a.NBTLookup.Any(l => l.KeyId == dyeKey);
    }

    private Expression<Func<IDbItem, bool>> ExplicitHexExpression(FilterArgs args, string stringVal)
    {
        var values = stringVal.Split(':').Last().Split(',')
            .Where(v => !string.IsNullOrWhiteSpace(v)).Select(hex => FromHex(hex)).ToHashSet();
        return IncludeExpression(args, values);
    }

    // nbt key set when a dye changed the color; see DyeItemFilter
    private const string DyeKey = "dye_item";

    private static bool Is(string label, string name) => label.Equals(name, StringComparison.OrdinalIgnoreCase);

    private HashSet<long> ToValues(IEnumerable<string> hexes) => hexes.Select(hex => FromHex(hex)).ToHashSet();

    /// <summary>
    /// Resolves the item default color for the query from the ItemId filter (the database path can
    /// not otherwise tell which item it is filtering). Runs once per query at expression build time.
    /// Returns null when the item / its default color can not be determined; the caller then simply
    /// omits the default from the exotic exclusion set.
    /// </summary>
    private long? GetItemDefaultColor(FilterArgs args)
    {
        if (itemService == null || itemDetails == null)
            return null;
        if (!args.TryGet("ItemId", out var idValue) || string.IsNullOrWhiteSpace(idValue))
            return null;
        // only a single concrete item has a well defined default color
        if (!int.TryParse(idValue, out var itemId))
            return null;
        var tag = itemDetails.TagLookup.FirstOrDefault(e => e.Value == itemId).Key;
        if (tag == null)
            return null;
        var (color, _) = itemService.GetDefaultColorAndCategory(tag);
        if (string.IsNullOrEmpty(color))
            return null;
        try
        {
            return FromHex(ExoticColorService.FormatHex(color));
        }
        catch
        {
            return null;
        }
    }

    private bool IsDyed(SaveAuction auction)
    {
        return auction.FlatenedNBT.ContainsKey(DyeKey);
    }

    private bool MatchesType(SaveAuction auction, HashSet<ColorType> types)
    {
        if (auction?.FlatenedNBT == null || auction.Tag == null)
            return false;
        // dyed items get their color from the dye, not from being naturally exotic
        if (IsDyed(auction))
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
        return new object[] { "Fairy" };
    }
}
public class CrystalColorFilter : ExoticColorFilter
{
    public override IEnumerable<object> OptionsGet(OptionValues options)
    {
        return new object[] { "Crystal" };
    }
}
