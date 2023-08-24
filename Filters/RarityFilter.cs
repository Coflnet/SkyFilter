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

        public override  Expression<Func<IDbItem, bool>> GetExpression(FilterArgs args)
        {
            var value = args.Get(this);
            if(!Enum.IsDefined(typeof(Tier),value))
                throw new CoflnetException("invalid_rarity", $"The passed rarity `{value}` is not valid");
            var rarity = Enum.Parse<Tier>(value);
            return a => (a as SaveAuction).Tier == rarity;
        }
    }
}
