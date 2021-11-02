using System;
using System.Collections.Generic;
using System.Linq;
using hypixel;

namespace Coflnet.Sky.Filter
{
    public class StarsFilter : GeneralFilter
    {
        public override FilterType FilterType => FilterType.EQUAL_NO_SEARCH;

        public override IEnumerable<object> Options => new object[] { "none", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" };

        public override Func<DBItem, bool> IsApplicable => item
            => (item?.Category == Category.WEAPON)
            || item.Category == Category.ARMOR;

        public override IQueryable<SaveAuction> AddQuery(IQueryable<SaveAuction> query, FilterArgs args)
        {
            var key = NBT.GetLookupKey("dungeon_item_level");
            var stringVal = args.Get(this);
            if (int.TryParse(stringVal, out int val))
                return query.Where(a => a.NBTLookup.Where(l => l.KeyId == key && l.Value == val).Any());
            return query.Where(a => !a.NBTLookup.Where(l => l.KeyId == key).Any());
        }
    }
}

