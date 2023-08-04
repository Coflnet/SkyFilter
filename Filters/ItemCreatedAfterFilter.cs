using System;
using System.Linq.Expressions;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter;

public class ItemCreatedAfterFilter : DateTimeFilter
{
    protected override Expression<Func<SaveAuction, bool>> GetComparison(DateTime timestamp)
    {
        return a => a.ItemCreatedAt > timestamp;
    }
}
