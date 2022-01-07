using System.Linq;
using Microsoft.EntityFrameworkCore;
using hypixel;

namespace Coflnet.Sky.Filter
{
    public abstract class SkinFilter : GeneralFilter
    {
        public override FilterType FilterType => FilterType.Equal;

        public override IQueryable<SaveAuction> AddQuery(IQueryable<SaveAuction> query, FilterArgs args)
        {
            var item = ItemDetails.Instance.GetItemIdForName(args.Get(this));
            var key = NBT.Instance.GetKeyId("skin");
            return query.Include(a => a.NBTLookup).Where(a => a.NBTLookup.Where(l => l.KeyId == key && l.Value == item).Any());
        }
    }
}
