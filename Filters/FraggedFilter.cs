using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter;

public class FraggedFilter : GeneralFilter
{
    public override FilterType FilterType => FilterType.BOOLEAN | FilterType.SIMPLE;
    public override IEnumerable<object> Options => new object[] { true, false };
    public override Func<Coflnet.Sky.Items.Client.Model.Item, bool> IsApplicable => i => i.Tag.StartsWith("STARRED_");
    public override IQueryable<IDbItem> AddQuery(IQueryable<IDbItem> query, FilterArgs args)
    {
        return query;
    }
    public override Expression<Func<IDbItem, bool>> GetExpression(FilterArgs args)
    {
        if (args.Get(this) == "true")
            return a => a.Tag.StartsWith("STARRED_");
        return a => !a.Tag.StartsWith("STARRED_");
    }
}