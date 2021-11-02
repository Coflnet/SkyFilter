using System.Collections.Generic;
using System.Linq;
using hypixel;

namespace Coflnet.Sky.Filter
{
    public class BinFilter : GeneralFilter
    {
        public override FilterType FilterType => FilterType.BOOLEAN;

        public override IEnumerable<object> Options => new object[] { "true", "false" };

        public override IQueryable<SaveAuction> AddQuery(IQueryable<SaveAuction> query, FilterArgs args)
        {
            if (args.Get(this) == "true")
                return query.Where(a => a.Bin);
            return query.Where(a => !a.Bin);

        }
    }
}

