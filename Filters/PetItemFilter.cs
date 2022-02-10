using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using hypixel;

namespace Coflnet.Sky.Filter
{
    public class PetItemFilter : PetFilter
    {
        public override FilterType FilterType => FilterType.Equal;
        public override IEnumerable<object> Options => ItemDetails.Instance.TagLookup.Keys.Where(k => k.StartsWith("PET_ITEM"))
            .Append("DWARF_TURTLE_SHELMET").Append("MINOS_RELIC");

        public override IQueryable<SaveAuction> AddQuery(IQueryable<SaveAuction> query, FilterArgs args)
        {
            var item = ItemDetails.Instance.GetItemIdForName(args.Get(this));
            var key = NBT.Instance.GetKeyId("heldItem");
            Console.WriteLine(item);
            Console.WriteLine(key);
            return query.Where(a => a.NBTLookup.Where(l => l.KeyId == key && l.Value == item).Any());
        }
    }
}
