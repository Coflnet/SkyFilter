using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using hypixel;

namespace Coflnet.Sky.Filter
{
    public abstract class GeneralFilter : IFilter
    {
        public string Name => this.GetType().Name.Replace("Filter", "");

        public virtual Func<DBItem, bool> IsApplicable => item => true;

        abstract public FilterType FilterType { get; }

        abstract public IEnumerable<object> Options { get; }

        public virtual IQueryable<SaveAuction> AddQuery(IQueryable<SaveAuction> query, FilterArgs args)
        {
            return query.Where(GetExpression(args));
        }

        public virtual IEnumerable<SaveAuction> Filter(IEnumerable<SaveAuction> items, FilterArgs args)
        {
            return items.Where(GetExpression(args).Compile());
        }

        public virtual Expression<Func<SaveAuction, bool>> GetExpression(FilterArgs args)
        {
            return a => true;
        }
    }
}
