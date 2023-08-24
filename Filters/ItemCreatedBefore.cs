using System;
using System.Linq.Expressions;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter;

public class ItemCreatedBeforeFilter : DateTimeFilter
{
    protected override Expression<Func<IDbItem, bool>> GetComparison(DateTime timestamp)
    {
        return a => (a as SaveAuction).ItemCreatedAt < timestamp;
    }
}
