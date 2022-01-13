using System;
using System.Collections.Generic;
using System.Linq;
using hypixel;

namespace Coflnet.Sky.Filter
{
    public class EverythingFilter : GeneralFilter
    {
        public override FilterType FilterType => FilterType.SIMPLE | FilterType.Equal | FilterType.BOOLEAN;
        public override IEnumerable<object> Options => Enum.GetNames(typeof(Tier));

        public override Func<DBItem, bool> IsApplicable => i => true;

        public override IQueryable<SaveAuction> AddQuery(IQueryable<SaveAuction> query, FilterArgs args)
        {
            return query;
        }
    }
}