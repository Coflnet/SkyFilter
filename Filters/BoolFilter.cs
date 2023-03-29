using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter
{
    public abstract class BoolFilter : GeneralFilter
    {
        public override FilterType FilterType => FilterType.BOOLEAN;

        public override IEnumerable<object> Options => new object[] { "true", "false" };

        public override Expression<Func<SaveAuction, bool>> GetExpression(FilterArgs args)
        {
            if (args.Get(this) == "true")
                return GetBool(args);
            return Inverse(GetBool(args));
        }
        public static Expression<Func<T, bool>> Inverse<T>(Expression<Func<T, bool>> e)
        {
            return Expression.Lambda<Func<T, bool>>(Expression.Not(e.Body), e.Parameters[0]);
        }

        public abstract Expression<Func<SaveAuction, bool>> GetBool(FilterArgs args);
    }
}

