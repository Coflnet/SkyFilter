using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter
{
    public class ReforgeFilter : GeneralFilter
    {
        public override FilterType FilterType => FilterType.Equal ;
        public override IEnumerable<object> Options => Enum.GetNames(typeof(ItemReferences.Reforge)).Where(e=>e != "Unkown").OrderBy(e=>e);

        public override Func<DBItem, bool> IsApplicable => item 
                => item.Category == Category.ACCESSORIES 
                || item.Category == Category.WEAPON 
                || item.Category == Category.ARMOR;

        public override Expression<Func<SaveAuction, bool>> GetExpression(FilterArgs args)
        {
            var rarity = Enum.Parse<ItemReferences.Reforge>(args.Get(this));
            return a => a.Reforge == rarity;
        }
    }
}
