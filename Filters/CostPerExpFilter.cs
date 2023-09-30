using System;
using System.Linq;
using System.Linq.Expressions;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter;

[FilterDescription("Cost per exp x+y base price")]
public class CostPerExpPlusBaseFilter : DoubleNumberFilter
{
    public override Expression<Func<IDbItem, double>> GetSelector(FilterArgs args)
    {
        var key = NBT.Instance.GetKeyId("exp");
        var parts = args.Get(this).Split("+");
        var remove = 0L;
        if(parts.Count() > 1)
            remove = NumberParser.Long(parts[1]);
        return a => (double)a.NBTLookup.Where(l => l.KeyId == key).Select(l => l.Value).FirstOrDefault() / (double)((a as SaveAuction).StartingBid - remove);
    }

    public override double GetUpperBound(FilterArgs args, double input)
    {
        return input;
    }

    protected override string GetValue(FilterArgs args)
    {
        return args.Get(this).Split("+").First();
    }
}
