using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter;

public abstract class DateTimeFilter : GeneralFilter
{
    public override FilterType FilterType => FilterType.HIGHER | FilterType.DATE;
    public override IEnumerable<object> Options => new object[] { new DateTime(2019, 6, 1), DateTime.Now + TimeSpan.FromDays(14) };

    public override Expression<Func<SaveAuction, bool>> GetExpression(FilterArgs args)
    {
        var timestamp = args.GetAsTimeStamp(this);
        return GetComparison(timestamp);
    }

    protected abstract Expression<Func<SaveAuction, bool>> GetComparison(DateTime timestamp);
}
