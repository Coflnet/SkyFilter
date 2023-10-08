using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Coflnet.Sky.Core;
using Coflnet.Sky.Items.Client.Model;

namespace Coflnet.Sky.Filter
{
    [FilterDescription("Filter auction history by unique id")]
    public class UIdFilter : GeneralFilter
    {
        public override FilterType FilterType => FilterType.Equal | FilterType.RANGE;
        public override IEnumerable<object> Options => new object[] { "000000000000", "ffffffffffff" };

        public override Func<Items.Client.Model.Item, bool> IsApplicable => item
                    => item?.Category != ItemCategory.UNKNOWN;


        public override Expression<Func<IDbItem, bool>> GetExpression(FilterArgs args)
        {
            var key = NBT.Instance.GetKeyId("uid");
            var val = NBT.UidToLong(args.Get(this));
            return a => a.NBTLookup.Where(l => l.KeyId == key && l.Value == val).Any();
        }
    }
}
