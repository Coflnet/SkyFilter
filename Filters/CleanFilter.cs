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

        private HashSet<string> okKeys;
        public CleanFilter()
        {
            okKeys = new string[]{"uid","exp","uuid", "spawnedFor", "bossId", "active",
                        "winning_bid", "is_shiny",
                        "type", "tier", "hideInfo", "candyUsed", "hideRightClick",
                        "cc", "color", // "copied color" and color itself are (usually) not applied individually
                        "count", "reforge", "abr", "name" // technically stored but not nbt
                        }
                        .Concat(FilterEngine.AttributeKeys).ToHashSet();
        }

        public override Func<Coflnet.Sky.Items.Client.Model.Item, bool> IsApplicable => a
            => a.Modifiers.Any(m => !okKeys.Contains(m.Slug));

        public override Expression<Func<IDbItem, bool>> GetExpression(FilterArgs args)
        {
            if(!args.TargetsDB)
                return a => (a as SaveAuction).FlatenedNBT.Where(n => !okKeys.Contains(n.Key)).Any();
            var ok = okKeys.Select(p => args.NbtIntance.GetKeyId(p)).ToHashSet();
            return a => !((a as SaveAuction).Enchantments.Any()) && !a.NBTLookup.Where(l => !ok.Contains(l.KeyId)).Any();
        }
    }
}
