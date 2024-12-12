using System;
using System.Linq.Expressions;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter;
public class EndAfterFilter : DateTimeFilter
{
    protected override Expression<Func<IDbItem, bool>> GetComparison(DateTime timestamp)
    {
        if (timestamp > DateTime.UtcNow - TimeSpan.FromDays(15) && timestamp < DateTime.UtcNow - TimeSpan.FromHours(1))
        {
            return a => (a as SaveAuction).End > timestamp && (a as SaveAuction).Id > 730_000_000;
        }
        return a => (a as SaveAuction).End > timestamp;
    }
}
