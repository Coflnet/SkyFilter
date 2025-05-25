using System;
using System.Linq;
using System.Linq.Expressions;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter
{
    [FilterDescription("The highest bid on dark auction items, aka full bid")]
    public class WinningBidFilter : NBTNumberFilter
    {
        protected override string PropName => "winning_bid";

        public override Expression<Func<IDbItem, long>> GetSelector(FilterArgs args)
        {
            if (!args.TargetsDB)
                return a => SelectNumber(a);
            var key = args.NbtIntance.GetKeyId(PropName);
            var extraKey = args.NbtIntance.GetKeyId("additional_coins");
            return a => a.NBTLookup.Where(l => l.KeyId == key).Select(l => l.Value).FirstOrDefault() + a.NBTLookup.Where(l => l.KeyId == extraKey).Select(l => l.Value).FirstOrDefault();
        }

        private long SelectNumber(IDbItem a)
        {
            if (a is not SaveAuction auction)
            {
                return 0;
            }
            var sum = 0L;
            foreach (var nbt in auction.FlatenedNBT)
            {
                if (nbt.Key == PropName || nbt.Key == "additional_coins")
                    sum += (long)double.Parse(nbt.Value);
            }
            return sum;
        }
    }
}

