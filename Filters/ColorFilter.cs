using System;
using System.Collections.Generic;
using System.Linq;
using hypixel;

namespace Coflnet.Sky.Filter
{
    public class ColorFilter : GeneralFilter
    {
        public override FilterType FilterType => FilterType.Equal | FilterType.RANGE;
        public override IEnumerable<object> Options => new object[] { "000000", "ffffff" };

        public override Func<DBItem, bool> IsApplicable => item
                    => item?.Category.HasFlag(Category.ARMOR) ?? false;

        public override IQueryable<SaveAuction> AddQuery(IQueryable<SaveAuction> query, FilterArgs args)
        {
            var key = NBT.Instance.GetKeyId("color");
            var stringVal = args.Get(this);
            long val = 0;
            if (stringVal.Contains(":"))
                val = NBT.GetColor(stringVal); 
            else // values are shifted a byte because the NBT.GetColor also mistakenly did that
                val = Convert.ToInt64(args.Get(this), 16)  << 8;
            return query.Where(a => a.NBTLookup.Where(l => l.KeyId == key && l.Value == val).Any());
        }
    }
}
