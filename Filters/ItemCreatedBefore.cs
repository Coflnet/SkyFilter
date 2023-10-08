using System;
using System.Linq.Expressions;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter;

[FilterDescription("Targets item creation time")]
public class ItemCreatedBeforeFilter : DateTimeFilter
{
    protected override Expression<Func<IDbItem, bool>> GetComparison(DateTime timestamp)
    {
        return a => (a as SaveAuction).ItemCreatedAt < timestamp;
    }
}
