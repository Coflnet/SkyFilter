using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Coflnet.Sky.Core;
using Coflnet.Sky.Items.Client.Model;

namespace Coflnet.Sky.Filter
{
    [FilterDescription("Filter auction history by unique id. This is the part of a uuid after the last dash (-)")]
    public class UIdFilter : GeneralFilter
    {
        public override FilterType FilterType => FilterType.Equal | FilterType.RANGE;
        public override IEnumerable<object> Options => new object[] { "000000000000", "ffffffffffff" };

        public override Func<Items.Client.Model.Item, bool> IsApplicable => item
                    => item?.Category != ItemCategory.UNKNOWN;


        public override Expression<Func<IDbItem, bool>> GetExpression(FilterArgs args)
        {
            if (!args.TargetsDB)
            {
                var stringVal = args.Get(this);
                return a => (a as SaveAuction).FlatenedNBT.ContainsKey("uid") && (a as SaveAuction).FlatenedNBT["uid"] == stringVal;
            }
            var key = args.NbtIntance.GetKeyId("uid");
            var val = NBT.UidToLong(args.Get(this));
            return a => a.NBTLookup.Where(l => l.KeyId == key && l.Value == val).Any();
        }
    }
}
