using System;
using System.Linq.Expressions;
using Coflnet.Sky.Core;
using System.Linq;
using System.Collections.Generic;

namespace Coflnet.Sky.Filter;
public class PricePerLevelFilter : NumberFilter
{
    private HashSet<string> relevantKeys = new HashSet<string>();
    public PricePerLevelFilter()
    {
        foreach (var item in Constants.AttributeKeys)
        {
            relevantKeys.Add(item);
        }
    }
    public override Func<Items.Client.Model.Item, bool> IsApplicable => a
        => a.Modifiers.Any(m => relevantKeys.Contains(m.Slug) || m.Slug.StartsWith("!ench"));

    public override Expression<Func<IDbItem, long>> GetSelector(FilterArgs args)
    {
        var engine = args.Engine;
        var filter = engine.GetFilter(args.Filters.First(f => f.Key != this.Name).Key) as NumberFilter;
        if (filter == null)
            throw new CoflnetException("invalid_filter", "Only attribute or enchantment filters can be used with price per level");
        var selector = filter.GetSelector(args);
        if (args.TargetsDB)
            throw new CoflnetException("not_supported", "The PricePerLevel filter is not supported for database history queries");
        return a => selector.Compile()(a) > 1 ? (a as SaveAuction).StartingBid / (long)(Math.Pow(2, (selector.Compile()(a) - 1))) : (a as SaveAuction).StartingBid;
    }
}
