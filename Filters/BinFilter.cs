using System;
using System.Linq.Expressions;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter;
public class BinFilter : BoolFilter
{
    public override Expression<Func<SaveAuction, bool>> GetBool(FilterArgs args)
    {
        return a => a.Bin;
    }
}

