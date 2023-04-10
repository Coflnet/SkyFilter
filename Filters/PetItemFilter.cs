using System;
using System.Collections.Generic;
using System.Linq;
using Coflnet.Sky.Core;
using System.Linq.Expressions;

namespace Coflnet.Sky.Filter;
public class PetItemFilter : PetFilter
{
    public override FilterType FilterType => FilterType.Equal;
    public override IEnumerable<object> Options => ItemDetails.Instance.TagLookup.Keys.Where(k => k.StartsWith("PET_ITEM"))
        .Append("DWARF_TURTLE_SHELMET")
        .Append("MINOS_RELIC")
        .Append("CROCHET_TIGER_PLUSHIE")
        .Prepend("any").Append("none");
    public override IEnumerable<object> OptionsGet(OptionValues options)
    {
        return  options.Options.GetValueOrDefault("heldItem", new List<string>())
            .Append(NBTFilter.None).Prepend(NBTFilter.Any)
            .Append("NOT_TIER_BOOST");
    }

    public override Expression<Func<SaveAuction, bool>> GetExpression(FilterArgs args)
    {
        var key = NBT.Instance.GetKeyId("heldItem");
        if (args.Get(this) == "NOT_TIER_BOOST")
        {
            var tierBoostId = ItemDetails.Instance.GetItemIdForTag("PET_ITEM_TIER_BOOST");
            return a => !a.NBTLookup.Where(l => l.KeyId == key && l.Value == tierBoostId).Any();
        }
        var item = ItemDetails.Instance.GetItemIdForTag(args.Get(this));
        if (args.Get(this) == NBTFilter.Any || string.IsNullOrEmpty(args.Get(this)))
            return a => a.NBTLookup.Where(l => l.KeyId == key).Any();
        if (args.Get(this) == NBTFilter.None)
            return a => !a.NBTLookup.Where(l => l.KeyId == key).Any();
        return a => a.NBTLookup.Where(l => l.KeyId == key && l.Value == item).Any();
    }
}
