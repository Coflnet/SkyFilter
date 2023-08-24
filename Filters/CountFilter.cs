using System;
using System.Linq.Expressions;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter
{
    public class CountFilter : NumberFilter
    {
        public override Expression<Func<IDbItem, long>> GetSelector(FilterArgs args)
        {
            return a => a.Count;
        }
    }
}
