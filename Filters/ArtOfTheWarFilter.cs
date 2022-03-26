using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter
{
    public class ArtOfTheWarFilter : GeneralFilter
    {
        public override FilterType FilterType => FilterType.Equal;
        public override IEnumerable<object> Options => new object[] { "yes" };

        public override Func<DBItem, bool> IsApplicable => item
                    => item?.Category.HasFlag(Category.WEAPON) ?? false;

        public override Expression<Func<SaveAuction, bool>> GetExpression(FilterArgs args)
        {
            var key = NBT.Instance.GetKeyId("art_of_war_count");
            return a => a.NBTLookup.Where(l => l.KeyId == key).Any();
        }
    }
}
