using System;
using System.Collections.Generic;
using System.Linq;
using Coflnet.Sky.Core;
using System.Linq.Expressions;

namespace Coflnet.Sky.Filter;
[FilterDescription("Item held by the pet")]
public class PetItemFilter : PetFilter
{
    public override FilterType FilterType => FilterType.Equal;
    public override IEnumerable<object> Options => ItemDetails.Instance.TagLookup.Keys.Where(k => k.StartsWith("PET_ITEM"))
        .Append("DWARF_TURTLE_SHELMET")
        .Append("MINOS_RELIC")
        .Append("CROCHET_TIGER_PLUSHIE")
        .Prepend(NBTFilter.Any).Append(NBTFilter.None);
    public override IEnumerable<object> OptionsGet(OptionValues options)
    {
        return  options.Options.GetValueOrDefault("heldItem", new List<string>())
            .Append(NBTFilter.None).Prepend(NBTFilter.Any)
            .Append("NOT_TIER_BOOST");
    }

    public override Expression<Func<IDbItem, bool>> GetExpression(FilterArgs args)
    {
        var key = NBT.Instance.GetKeyId("heldItem");
        if (args.Get(this) == "NOT_TIER_BOOST")
        {
            var tierBoostId = ItemDetails.Instance.GetItemIdForTag("PET_ITEM_TIER_BOOST");
            return a => !a.NBTLookup.Where(l => l.KeyId == key && l.Value == tierBoostId).Any();
        }
        if (args.Get(this) == NBTFilter.Any || string.IsNullOrEmpty(args.Get(this)))
            return a => a.NBTLookup.Where(l => l.KeyId == key).Any();
        if (args.Get(this) == NBTFilter.None)
            return a => !a.NBTLookup.Where(l => l.KeyId == key).Any();
        var item = ItemDetails.Instance.GetItemIdForTag(args.Get(this));
        return a => a.NBTLookup.Where(l => l.KeyId == key && l.Value == item).Any();
    }
}
