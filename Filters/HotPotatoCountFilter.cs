using System;
using System.Collections.Generic;
using System.Linq;
using hypixel;

namespace Coflnet.Sky.Filter
{
    public class HotPotatoCountFilter : GeneralFilter
    {
        public override FilterType FilterType => FilterType.Equal;

        public override IEnumerable<object> Options => new object[] { "none", "1-9", "10", "15" };

        public override Func<DBItem, bool> IsApplicable => item
            => (item?.Category == Category.WEAPON)
            || item.Category == Category.ARMOR;

        public override IQueryable<SaveAuction> AddQuery(IQueryable<SaveAuction> query, FilterArgs args)
        {
            var key = NBT.GetLookupKey("hpc");
            if (args.Get(this) == "none")
                return query.Where(a => !a.NBTLookup.Where(l => l.KeyId == key).Any());
            if (args.Get(this) == "1-9")
                return query.Where(a => !a.NBTLookup.Where(l => l.KeyId == key && l.Value > 0 && l.Value < 10).Any());
            var val = args.GetAsLong(this);
            return query.Where(a => a.NBTLookup.Where(l => l.KeyId == key && l.Value == val).Any());
        }
    }
}

