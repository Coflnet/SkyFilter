using System;
using System.Linq.Expressions;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter;

[FilterDescription("Has item sold (ended and at least one bid or bin)")]
public class SoldFilter : BoolFilter
{
    public override Expression<Func<IDbItem, bool>> GetBool(FilterArgs args)
    {
        return a => (a as SaveAuction).End < DateTime.UtcNow && (a as SaveAuction).HighestBidAmount > 0;
    }
}
