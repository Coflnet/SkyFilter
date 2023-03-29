using System;
using System.Linq.Expressions;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter;

public class SoldFilder : BoolFilter
{
    public override Expression<Func<SaveAuction, bool>> GetBool(FilterArgs args)
    {
        return a => a.End < DateTime.UtcNow && a.HighestBidAmount > 0;
    }
}
