using System;
using System.Linq;
using System.Linq.Expressions;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter;

[FilterDescription("Price per unit (for stackable items)")]
public class PricePerUnitFilter : NBTNumberFilter
{
    protected override string PropName => "count";
    public override Func<Items.Client.Model.Item, bool> IsApplicable => a
        => a.Modifiers.Any(m => m.Slug == PropName && m.Value.Count() > 1);

    public override Expression<Func<IDbItem, long>> GetSelector(FilterArgs args)
    {
        return a => (a as SaveAuction).StartingBid / (a.Count == 0 ? 1 : a.Count);
    }
}