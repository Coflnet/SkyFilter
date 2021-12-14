using System;
using System.Collections.Generic;
using System.Linq;
using hypixel;

namespace Coflnet.Sky.Filter
{
    public class ArtOfTheWarFilter : GeneralFilter
    {
        public override FilterType FilterType => FilterType.Equal;
        public override IEnumerable<object> Options => new object[] { "yes" };

        public override Func<DBItem, bool> IsApplicable => item
                    => item?.Category.HasFlag(Category.WEAPON) ?? false;

        public override IQueryable<SaveAuction> AddQuery(IQueryable<SaveAuction> query, FilterArgs args)
        {
            var key = NBT.Instance.GetKeyId("art_of_war_count");
            return query.Where(a => a.NBTLookup.Where(l => l.KeyId == key).Any());
        }
    }
}
