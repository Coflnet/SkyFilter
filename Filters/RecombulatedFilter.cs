using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter
{
    public class RecombobulatedFilter : BoolNbtFilter
    {
        public override FilterType FilterType => FilterType.BOOLEAN;

        public override IEnumerable<object> Options => new object[] { "true", "false" };

        public override string Key => "rarity_upgrades";

        public override Expression<Func<IDbItem, bool>> GetExpression(FilterArgs args)
        {
            var key = NBT.Instance.GetKeyId("rarity_upgrades");
            var stringVal = args.Get(this);
            if (args.Get(this) == "true")
                return a => a.NBTLookup.Where(l => l.KeyId == key).Any();
            return a => !a.NBTLookup.Where(l => l.KeyId == key).Any();
        }
    }
}

