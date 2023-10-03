using System;
using System.Linq.Expressions;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter;
public class ReforgeFilter : AlwaysPresentNbtFiler
{
    public override FilterType FilterType => FilterType.Equal;
    protected override string PropName => "reforge";

    public override Expression<Func<IDbItem, bool>> GetExpression(FilterArgs args)
    {
        var rarity = Enum.Parse<ItemReferences.Reforge>(args.Get(this));
        return a => (a as SaveAuction).Reforge == rarity;
    }
}
