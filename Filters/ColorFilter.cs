using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter
{
    public class ColorFilter : GeneralFilter
    {
        public override FilterType FilterType => FilterType.Equal | FilterType.RANGE;
        public override IEnumerable<object> Options => new object[] { "000000", "ffffff" };

        public override Func<Coflnet.Sky.Items.Client.Model.Item, bool> IsApplicable => item
                    => item?.Modifiers.Any(m => m.Slug == "color") ?? false;

        public override Expression<Func<SaveAuction, bool>> GetExpression(FilterArgs args)
        {
            var key = NBT.Instance.GetKeyId("color");
            var stringVal = args.Get(this);
            long val = 0;
            if (stringVal.Contains(":"))
                val = NBT.GetColor(stringVal);
            else // values are shifted a byte because the NBT.GetColor also mistakenly did that
                val = FromHex(args.Get(this));
            //                val |=((long)0xFFFFFFFFF000000<<8);

            return a => a.NBTLookup.Where(l => l.KeyId == key && l.Value == val).Any();
        }

        public long FromHex(string args)
        {
            return Convert.ToInt32(args, 16) << 8;
        }
        public string ToHex(string dec)
        {
            var color = NBT.GetColor(dec);
            color = color >> 8 & 0xFFFFFF;
            return color.ToString("X6");
        }
    }
}
