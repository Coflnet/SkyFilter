using System;
using System.Collections.Generic;
using System.Linq;
using hypixel;

namespace Coflnet.Sky.Filter
{
    public class CapturedPlayerFilter : GeneralFilter
    {
        public override FilterType FilterType => FilterType.Equal | FilterType.TEXT | FilterType.PLAYER_WITH_RANK;
        public override IEnumerable<object> Options => new object[] { "", "" };

        public override Func<DBItem, bool> IsApplicable => item
                    => item?.Tag == "CAKE_SOUL";

        public override IQueryable<SaveAuction> AddQuery(IQueryable<SaveAuction> query, FilterArgs args)
        {
            var key = NBT.Instance.GetKeyId("captured_player");
            if(string.IsNullOrEmpty(args.Get(this)))
                return query.Where(a => !a.NBTLookup.Where(l => l.KeyId == key).Any());
            var val = NBT.Instance.GetValueId(key,args.Get(this));
            return query.Where(a => a.NBTLookup.Where(l => l.KeyId == key && l.Value == val).Any());
        }
    }
}
