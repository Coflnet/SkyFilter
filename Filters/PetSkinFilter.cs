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
            var key = NBT.Instance.GetKeyId("skin");
            if (args.Get(this) == "any")
            {
                if (args.TargetsDB)
                    return query.Where(a => a.NBTLookup.Where(l => l.KeyId == key).Any());
                return query.Where(a => a.FlatenedNBT.ContainsKey("skin"));
            }
            var item = ItemDetails.Instance.GetItemIdForName(args.Get(this));
            if (args.TargetsDB)
                return query.Where(a => a.NBTLookup.Where(l => l.KeyId == key && l.Value == item).Any());
            return query.Where(a => a.FlatenedNBT.GetValueOrDefault("skin") == args.Get(this));

        }
    }
}
