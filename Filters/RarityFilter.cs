using System;
using System.Linq.Expressions;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter;
[FilterDescription("aka Tier (Rare, Epic, etc)")]
public class RarityFilter : AlwaysPresentNbtFiler
{
    public override FilterType FilterType => FilterType.Equal;
    public override Func<Items.Client.Model.Item, bool> IsApplicable => a
            => base.IsApplicable(a) || PetFilter.IsPet(a);
    protected override string PropName => "tier";

    public override Expression<Func<IDbItem, bool>> GetExpression(FilterArgs args)
    {
        var value = args.Get(this);
        if (value == null)
            return a => (a as SaveAuction).Tier == Tier.LEGENDARY;
        if (!Enum.IsDefined(typeof(Tier), value))
            throw new CoflnetException("invalid_rarity", $"The passed rarity `{value}` is not valid");
        var rarity = Enum.Parse<Tier>(value);
        return a => (a as SaveAuction).Tier == rarity;
    }
}