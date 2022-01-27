using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using hypixel;

namespace Coflnet.Sky.Filter
{
    public class BinFilter : GeneralFilter
    {
        public override FilterType FilterType => FilterType.BOOLEAN;

        public override IEnumerable<object> Options => new object[] { "true", "false" };

        public override Expression<Func<SaveAuction, bool>> GetExpression(FilterArgs args)
        {
            if (args.Get(this) == "true")
                return a => a.Bin;
            return a => !a.Bin;
        }
    }
}

