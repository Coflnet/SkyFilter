using System;
using System.Linq;
using System.Linq.Expressions;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter;

[FilterDescription("Cost per exp")]
public class CostPerExpFilter : DoubleNumberFilter
{
    public override Expression<Func<IDbItem, double>> GetSelector(FilterArgs args)
    {
        var key = NBT.Instance.GetKeyId("exp");
        return a => (double)a.NBTLookup.Where(l => l.KeyId == key).Select(l => l.Value).FirstOrDefault() / (double)(a as SaveAuction).StartingBid;
    }

    public override double GetUpperBound(FilterArgs args, double input)
    {
        return input;
    }
}
