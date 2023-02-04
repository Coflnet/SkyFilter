using System;
using System.Linq.Expressions;
using Coflnet.Sky.Core;
using System.Linq;

namespace Coflnet.Sky.Filter;
public class PricePerLevelFilter : NumberFilter
{
    public override Expression<Func<SaveAuction, long>> GetSelector(FilterArgs args)
    {
        var engine = args.Engine;
        var filter = engine.GetFilter(args.Filters.First(f=>f.Key != this.Name).Key) as NumberFilter;
        if (filter == null)
            throw new CoflnetException("invalid_filter", "Only attribute or enchantment filters can be used with price per level");
        var selector = filter.GetSelector(args);
        if(args.TargetsDB)
            throw new CoflnetException("not_supported","The PricePerLevel filter is not supported for database history queries");
        return a => selector.Compile()(a) > 1 ? a.StartingBid / (long)(Math.Pow(2, (selector.Compile()(a) - 1))) : a.StartingBid;
    }
}
