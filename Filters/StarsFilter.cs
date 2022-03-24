using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using hypixel;

namespace Coflnet.Sky.Filter
{
    public class StarsFilter : NBTNumberFilter
    {
        public override IEnumerable<object> Options => new object[] { "none", "10" };

        public override Func<DBItem, bool> IsApplicable => item
            => (item?.Category == Category.WEAPON)
            || item.Category == Category.ARMOR;

        protected override string PropName => "dungeon_item_level";

        public override Expression<Func<SaveAuction, bool>> GetExpression(FilterArgs args)
        {
            // backwards compatable with `none`
            var key = NBT.Instance.GetKeyId(PropName);
            var stringVal = args.Get(this);
            if (int.TryParse(stringVal, out int val) || ContainsRangeRequest(stringVal))
                return base.GetExpression(args);
            return a => !a.NBTLookup.Where(l => l.KeyId == key).Any();
        }
    }
}

