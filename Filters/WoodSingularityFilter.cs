using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter
{
    public class WoodSingularityFilter : GeneralFilter
    {
        public override FilterType FilterType => FilterType.Equal;
        public override IEnumerable<object> Options => new object[] { "yes" };

        public override Func<DBItem, bool> IsApplicable => item
                    => new string[] { "TACTICIAN_SWORD", "SWORD_OF_REVELATIONS", "WOOD_SWORD" }.Contains(item?.Tag);

        public override Expression<Func<SaveAuction, bool>> GetExpression(FilterArgs args)
        {
            var key = NBT.Instance.GetKeyId("wood_singularity_count");
            return a => a.NBTLookup.Where(l => l.KeyId == key).Any();
        }
    }
}
