using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter;

[FilterDescription("Color of a Cake Soul, derived from its durability value")]
public class CakeSoulColorFilter : NBTFilter
{
    protected override string PropName => "soul_durability";

    // The soul_durability value (0-15) maps to a dyed color, see the item's leather armor color meta.
    private static readonly string[] ColorsByDurability =
    {
        "Black", "Red", "Dark Green", "Brown", "Blue", "Purple", "Cyan", "Light Gray",
        "Gray", "Pink", "Lime", "Yellow", "Light Blue", "Magenta", "Orange", "White"
    };

    private static readonly Dictionary<string, long> DurabilityByColor =
        ColorsByDurability
            .Select((name, index) => (name, index))
            .ToDictionary(x => x.name, x => (long)x.index, StringComparer.OrdinalIgnoreCase);

    public override IEnumerable<object> OptionsGet(OptionValues options)
    {
        return ColorsByDurability.Cast<object>();
    }

    public override Expression<Func<IDbItem, bool>> GetExpression(FilterArgs args)
    {
        var stringValue = args.Get(this);
        // Accept both the color name and the raw durability number.
        if (!DurabilityByColor.TryGetValue(stringValue, out var durability)
            && !long.TryParse(stringValue, out durability))
            return a => false;

        if (!args.TargetsDB)
            return a => (a as SaveAuction).FlatenedNBT.Where(n => n.Key == PropName)
                .Select(n => (long)double.Parse(n.Value)).Any(v => v == durability);

        var key = args.NbtIntance.GetKeyId(PropName);
        return a => a.NBTLookup.Any(l => l.KeyId == key && l.Value == durability);
    }
}
