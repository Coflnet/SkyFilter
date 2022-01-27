using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using hypixel;

namespace Coflnet.Sky.Filter
{
    public class ColorFilter : GeneralFilter
    {
        public override FilterType FilterType => FilterType.Equal | FilterType.RANGE;
        public override IEnumerable<object> Options => new object[] { "000000", "ffffff" };

        public override Func<DBItem, bool> IsApplicable => item
                    => item?.Category.HasFlag(Category.ARMOR) ?? false;

        public override Expression<Func<SaveAuction, bool>> GetExpression(FilterArgs args)
        {
            var key = NBT.Instance.GetKeyId("color");
            var stringVal = args.Get(this);
            long val = 0;
            if (stringVal.Contains(":"))
                val = NBT.GetColor(stringVal); 
            else // values are shifted a byte because the NBT.GetColor also mistakenly did that
                val = Convert.ToInt64(args.Get(this), 16)  << 8;
//                val |=((long)0xFFFFFFFFF000000<<8);
                
            return a => a.NBTLookup.Where(l => l.KeyId == key && l.Value == val).Any();
        }
    }
}
