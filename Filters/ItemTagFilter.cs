using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter;

public class ItemTagFilter : GeneralFilter
{
    public override FilterType FilterType => FilterType.Equal;
    public override IEnumerable<object> Options => new object[] {  };
    public override Func<Items.Client.Model.Item, bool> IsApplicable => i => false;
    public override Expression<Func<IDbItem, bool>> GetExpression(FilterArgs args)
    {
        var tag = args.Get(this);
        return args => args.Tag == tag;
    }
}
