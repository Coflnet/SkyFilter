using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter
{
    public class CleanFilter : GeneralFilter
    {
        public override FilterType FilterType => FilterType.BOOLEAN;

        public override IEnumerable<object> Options => new string[] { "yes" };

        public override Expression<Func<IDbItem, bool>> GetExpression(FilterArgs args)
        {
            var okStrings = new string[]{"uid","exp","uuid", "spawnedFor", "bossId", "active",
                        "winning_bid",
                        "type", "tier", "hideInfo", "candyUsed", "hideRightClick"}.Concat(FilterEngine.AttributeKeys);
            var ok = okStrings.Select(p => NBT.Instance.GetKeyId(p)).ToHashSet();
            return a => !((a as SaveAuction).Enchantments.Any()) && !a.NBTLookup.Where(l => !ok.Contains(l.KeyId)).Any();
        }
    }
}
