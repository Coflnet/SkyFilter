using System;
using System.Linq;
using System.Linq.Expressions;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter
{
    public abstract class NBTItemFilter : NBTFilter
    {
        public override Expression<Func<SaveAuction, bool>> GetExpression(FilterArgs args)
        {
            var stringValue = args.Get(this);
            if(!args.TargetsDB)
                return a => a.FlatenedNBT.Where(v => v.Key == PropName && v.Value == stringValue).Any();
            var key = NBT.Instance.GetKeyId(PropName);
            var value = ItemDetails.Instance.GetItemIdForTag(stringValue);
            return a => a.NBTLookup.Where(l => l.KeyId == key && l.Value == value).Any();
        }
    }
}

