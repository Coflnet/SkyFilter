using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter
{
    public class EverythingFilter : GeneralFilter
    {
        public override FilterType FilterType => FilterType.SIMPLE | FilterType.Equal | FilterType.BOOLEAN;
        public override IEnumerable<object> Options => new object[] { "true" };

        public override Func<Coflnet.Sky.Items.Client.Model.Item, bool> IsApplicable => i => true;

        public override IQueryable<IDbItem> AddQuery(IQueryable<IDbItem> query, FilterArgs args)
        {
            return query;
        }

        public override Expression<Func<IDbItem, bool>> GetExpression(FilterArgs args)
        {
            return (a) => true;
        }
    }
}
