using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter
{
    public class ColorFilter : NBTFilter
    {
        public override FilterType FilterType => FilterType.Equal;
        protected override string PropName => "color";

        public override Expression<Func<IDbItem, bool>> GetExpression(FilterArgs args)
        {
            var stringVal = args.Get(this);
            var val = new List<long>();
            if (stringVal.Contains(":"))
                val.Add(NBT.GetColor(stringVal));
            else // values are shifted a byte because the NBT.GetColor also mistakenly did that
                val.AddRange(stringVal.Split(',', ' ').Select(v => FromHex(v)));
            //                val |=((long)0xFFFFFFFFF000000<<8);
            if(!args.TargetsDB)
                return a => (a as SaveAuction).FlatenedNBT.Where(n => n.Key == PropName).Select(n => NBT.GetColor(n.Value)).Intersect(val).Any();
            var key = NBT.Instance.GetKeyId("color");
            return a => a.NBTLookup.Where(l => l.KeyId == key && val.Contains(l.Value)).Any();
        }

        public long FromHex(string args)
        {
            return Convert.ToInt32(args, 16) << 8;
        }
        public string ToHex(string dec)
        {
            var color = NBT.GetColor(dec.TrimStart('#'));
            color = color >> 8 & 0xFFFFFF;
            return color.ToString("X6");
        }
    }
}
