using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Coflnet.Sky.Core;
using Microsoft.EntityFrameworkCore;

namespace Coflnet.Sky.Filter;
[FilterDescription("Text the item name should contain.")]
public class ItemNameContainsFilter : GeneralFilter
{
    public override FilterType FilterType => FilterType.Equal | FilterType.RANGE;
    public override IEnumerable<object> Options => new object[] { "" };

    public override Expression<System.Func<IDbItem, bool>> GetExpression(FilterArgs args)
    {
        var text = args.Get(this);
        if (!args.TargetsDB)
            return a => a.ItemName.Contains(text, System.StringComparison.OrdinalIgnoreCase);
        var securedText = Regex.Replace(text, @"[^-a-zA-Z0-9 ⚚✪]", "");
        return a => EF.Functions.Like(a.ItemName, $"%{securedText}%");
    }
}
