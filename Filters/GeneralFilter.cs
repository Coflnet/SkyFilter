using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Coflnet.Sky.Core;
using System.Threading.Tasks;

namespace Coflnet.Sky.Filter
{
    public abstract class GeneralFilter : IFilter
    {
        public virtual string Name => this.GetType().Name.Replace("Filter", "");

        public virtual Func<DBItem, bool> IsApplicable => item => true;

        abstract public FilterType FilterType { get; }

        abstract public IEnumerable<object> Options { get; }

        public virtual IQueryable<SaveAuction> AddQuery(IQueryable<SaveAuction> query, FilterArgs args)
        {
            var exp = GetExpression(args);
            if(exp == null)
                return query;
            return query.Where(exp);
        }

        public virtual IEnumerable<SaveAuction> Filter(IEnumerable<SaveAuction> items, FilterArgs args)
        {
            return items.Where(GetExpression(args).Compile());
        }

        public virtual Expression<Func<SaveAuction, bool>> GetExpression(FilterArgs args)
        {
            return a => true;
        }
        public virtual Task LoadData(IServiceProvider provider)
        {
            return Task.CompletedTask;
        }
    }
}
