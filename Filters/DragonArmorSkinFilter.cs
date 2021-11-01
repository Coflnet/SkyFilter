using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using hypixel;
using System;

namespace Coflnet.Sky.Filter
{
    public class DragonArmorSkinFilter : GeneralFilter
    {
        public override FilterType FilterType => FilterType.Equal;

        public override IEnumerable<object> Options => ItemDetails.Instance.TagLookup.Keys.Where(k => k.EndsWith("_SHIMMER") || k.EndsWith("_BABY"));

        public Func<DBItem, bool> IsApplicable = item => item.Tag.EndsWith("_DRAGON_HELMET");

        public override IQueryable<SaveAuction> AddQuery(IQueryable<SaveAuction> query, FilterArgs args)
        {
            var item = ItemDetails.Instance.GetItemIdForName(args.Get(this));
            var key = NBT.GetLookupKey("skin");
            return query.Include(a => a.NBTLookup).Where(a => a.NBTLookup.Where(l => l.KeyId == key && l.Value == item).Any());
        }
    }
}
