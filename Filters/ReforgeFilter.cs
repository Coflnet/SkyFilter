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

        public override Func<Coflnet.Sky.Items.Client.Model.Item, bool> IsApplicable => item 
                => true;

        public override Expression<Func<IDbItem, bool>> GetExpression(FilterArgs args)
        {
            var rarity = Enum.Parse<ItemReferences.Reforge>(args.Get(this));
            return a => (a as SaveAuction).Reforge == rarity;
        }
    }
}
