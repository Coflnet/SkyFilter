using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter
{
    public class StarsFilter : NBTNumberFilter
    {
        protected override string PropName => "dungeon_item_level";

        public override Expression<Func<IDbItem, bool>> GetExpression(FilterArgs args)
        {
            // backwards compatable with `none`
            var key = NBT.Instance.GetKeyId(PropName);
            var stringVal = args.Get(this);
            var newKey = NBT.Instance.GetKeyId("upgrade_level");
            if (int.TryParse(stringVal, out int val) || ContainsRangeRequest(stringVal))
                return base.GetExpression(args);
            return a => !a.NBTLookup.Where(l => l.KeyId == key || l.KeyId == newKey).Any();
        }

        public override Expression<Func<IDbItem, long>> GetSelector(FilterArgs args)
        {
            var key = NBT.Instance.GetKeyId(PropName);
            var key2 = NBT.Instance.GetKeyId("upgrade_level");
            return a => a.NBTLookup.Where(l => l.KeyId == key || l.KeyId == key2).OrderByDescending(l=>l.KeyId == key2 ? 1 : 0).Select(l => l.Value).FirstOrDefault();
        }
    }
}

