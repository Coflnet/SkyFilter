using System;
using System.Collections.Generic;
using System.Linq;
using Coflnet.Sky.Core;
using System.Linq.Expressions;

namespace Coflnet.Sky.Filter;
[FilterDescription("Item held by the pet")]
public class PetItemFilter : NBTItemFilter
{
    protected override string PropName => "heldItem";

    public override IEnumerable<object> OptionsGet(OptionValues options)
    {
        return options.Options.GetValueOrDefault("heldItem", new List<string>())
            .Append(None).Prepend(Any)
            .Append("NOT_TIER_BOOST");
    }

    public override Expression<Func<IDbItem, bool>> GetExpression(FilterArgs args)
    {
        if (args.Get(this) == "NOT_TIER_BOOST")
        {
            if (!args.TargetsDB)
                return a => !(a as SaveAuction).FlatenedNBT.Where(v => v.Key == PropName && v.Value == "PET_ITEM_TIER_BOOST").Any() && ItemCheck()(a as SaveAuction);
            var tierBoostId = ItemDetails.Instance.GetItemIdForTag("PET_ITEM_TIER_BOOST");
            var key = NBT.Instance.GetKeyId("heldItem");
            return a => !a.NBTLookup.Where(l => l.KeyId == key && l.Value == tierBoostId).Any();
        }
        return base.GetExpression(args);
    }
}
