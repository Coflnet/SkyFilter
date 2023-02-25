using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter
{
    [FilterDescription("aka Tier (Rare, Epic, etc)")]
    public class RarityFilter : GeneralFilter
    {
        public override FilterType FilterType => FilterType.Equal;
        public override IEnumerable<object> Options => Enum.GetNames(typeof(Tier));

        public override IQueryable<SaveAuction> AddQuery(IQueryable<SaveAuction> query, FilterArgs args)
        {
            var rarity = Enum.Parse<Tier>(args.Get(this));
            return query.Where(a => a.Tier == rarity);
        }

        public override  Expression<Func<SaveAuction, bool>> GetExpression(FilterArgs args)
        {
            var rarity = Enum.Parse<Tier>(args.Get(this));
            return a => a.Tier == rarity;
        }
    }
}
