using System;
using System.Linq.Expressions;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter
{
    [FilterDescription("aka item Cost")]
    public class StartingBidFilter : NumberFilter
    {
        public override Expression<Func<IDbItem, long>> GetSelector(FilterArgs args)
        {
            return a => (a as SaveAuction).StartingBid;
        }
    }
}
