using System.Collections.Generic;
using System.Linq;
using hypixel;

namespace Coflnet.Sky.Filter
{
    public class CandyFilter : PetFilter
    {
        public override FilterType FilterType => FilterType.Equal;

        public override IEnumerable<object> Options => new object[] { "none", "any", "1", "2", "3", "4", "5", "6", "/", "8", "9", "10" };

        public override IQueryable<SaveAuction> AddQuery(IQueryable<SaveAuction> query, FilterArgs args)
        {
            var key = NBT.GetLookupKey("candyUsed");
            var stringVal = args.Get(this);
            if (int.TryParse(stringVal, out int val))
                return query.Where(a => a.NBTLookup.Where(l => l.KeyId == key && l.Value == val).Any());
            if (stringVal == "any")
                return query.Where(a => a.NBTLookup.Where(l => l.KeyId == key && l.Value != 0).Any());
            return query.Where(a => a.NBTLookup.Where(l => l.KeyId == key && l.Value == 0).Any());
        }
    }
}

