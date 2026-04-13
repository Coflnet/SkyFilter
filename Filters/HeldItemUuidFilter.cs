using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter;

[FilterDescription("Filter by the UUID of the pet's held item (last 12 hex chars of heldItemUuid)")]
public class HeldItemUuidFilter : GeneralFilter
{
    public override FilterType FilterType => FilterType.Equal;
    public override IEnumerable<object> Options => new object[] { "000000000000", "ffffffffffff" };

    public override Func<Items.Client.Model.Item, bool> IsApplicable => a
                => a.Modifiers.Any(m => m.Slug == "heldItemUuid");

    public override Expression<Func<IDbItem, bool>> GetExpression(FilterArgs args)
    {
        if (!args.TargetsDB)
        {
            var stringVal = args.Get(this);
            return a => (a as SaveAuction).FlatenedNBT.ContainsKey("heldItemUuid")
                && (a as SaveAuction).FlatenedNBT["heldItemUuid"].EndsWith(stringVal);
        }
        var key = args.NbtIntance.GetKeyId("heldItemUuid");
        var val = NBT.UidToLong(args.Get(this));
        return a => a.NBTLookup.Where(l => l.KeyId == key && l.Value == val).Any();
    }
}
