using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter
{
    public class RecombobulatedFilter : GeneralFilter
    {
        public override FilterType FilterType => FilterType.BOOLEAN;

        public override IEnumerable<object> Options => new object[] { "true", "false" };

        public override Func<DBItem, bool> IsApplicable => item
            => (item?.Category == Category.WEAPON)
            || item.Category == Category.ARMOR
            || item.Category == Category.ACCESSORIES;


        public override Expression<Func<SaveAuction, bool>> GetExpression(FilterArgs args)
        {
            var key = NBT.Instance.GetKeyId("rarity_upgrades");
            var stringVal = args.Get(this);
            if (args.Get(this) == "true")
                return a => a.NBTLookup.Where(l => l.KeyId == key).Any();
            return a => !a.NBTLookup.Where(l => l.KeyId == key).Any();
        }
    }
}

