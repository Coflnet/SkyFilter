using System;
using System.Collections.Generic;
using System.Linq;
using hypixel;

namespace Coflnet.Sky.Filter
{
    public class JyrreMaxFilter : GeneralFilter
    {
        public override FilterType FilterType => FilterType.HIGHER;
        public override IEnumerable<object> Options => new object[] { "132600" };

        public override Func<DBItem, bool> IsApplicable => item
                    => item?.Tag == "BOTTLE_OF_JYRRE";

        public override IQueryable<SaveAuction> AddQuery(IQueryable<SaveAuction> query, FilterArgs args)
        {
            var key = NBT.Instance.GetKeyId("bottle_of_jyrre_seconds");
            long val = args.GetAsLong(this);
            if(val == 0)
                val = 132600;
            return query.Where(a => a.NBTLookup.Where(l => l.KeyId == key && l.Value >= val).Any());
        }
    }
}
