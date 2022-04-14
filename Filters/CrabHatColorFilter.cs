using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter
{
    public class CrabHatColorFilter : GeneralFilter
    {
        public override FilterType FilterType => FilterType.Equal | FilterType.SIMPLE;
        public override IEnumerable<object> Options => new object[] { "red", "orange", "yellow", "lime", "green", "aqua", "purple", "pink", "black" };

        public override Func<DBItem, bool> IsApplicable => item
                    => item.Tag == "PARTY_HAT_CRAB";

        public override Expression<Func<SaveAuction, bool>> GetExpression(FilterArgs args)
        {
            var key = NBT.Instance.GetKeyId("party_hat_color");
            var value = NBT.Instance.GetValueId(key, args.Get(this));
            return a => a.NBTLookup.Where(l => l.KeyId == key && l.Value == value).Any();
        }
    }
}
