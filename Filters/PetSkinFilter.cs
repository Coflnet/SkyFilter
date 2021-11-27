using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using hypixel;

namespace Coflnet.Sky.Filter
{
    public class PetSkinFilter : PetFilter
    {
        public override FilterType FilterType => FilterType.Equal;

        public override IEnumerable<object> Options => ItemDetails.Instance.TagLookup.Keys.Where(k => k.StartsWith("PET_SKIN")).Prepend("any");

        public override IQueryable<SaveAuction> AddQuery(IQueryable<SaveAuction> query, FilterArgs args)
        {
            var key = NBT.GetLookupKey("skin");
            if (args.Get(this) == "any")
                return query.Where(a => a.NBTLookup.Where(l => l.KeyId == key).Any());
            var item = ItemDetails.Instance.GetItemIdForName(args.Get(this));
            return query.Where(a => a.NBTLookup.Where(l => l.KeyId == key && l.Value == item).Any());
        }
    }
}
