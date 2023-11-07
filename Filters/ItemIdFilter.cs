using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter;
/// <summary>
/// Internal filter to transmit an item id
/// </summary>
public class ItemIdFilter : GeneralFilter
{
    public override FilterType FilterType => FilterType.Equal | FilterType.NUMERICAL;
    public override IEnumerable<object> Options => new object[] { 1, 1000 };

    public override Func<Items.Client.Model.Item, bool> IsApplicable => i => false;

    public override IQueryable<IDbItem> AddQuery(IQueryable<IDbItem> query, FilterArgs args)
    {
        return query;
    }

    public override Expression<Func<IDbItem, bool>> GetExpression(FilterArgs args)
    {
        return a => true;
    }

}
