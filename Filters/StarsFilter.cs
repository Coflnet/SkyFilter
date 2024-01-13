using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter
{
    public class StarsFilter : NBTNumberFilter
    {
        protected override string PropName => "upgrade_level";

        public override Expression<Func<IDbItem, bool>> GetExpression(FilterArgs args)
        {
            // backwards compatable with `none` -- could be dropped
            var stringVal = args.Get(this);
            if (int.TryParse(stringVal, out int val) || ContainsRangeRequest(stringVal))
                return base.GetExpression(args);
            var key = NBT.Instance.GetKeyId(PropName);
            var oldKey = NBT.Instance.GetKeyId("dungeon_item_level");
            return a => !a.NBTLookup.Where(l => l.KeyId == key || l.KeyId == oldKey).Any();
        }

        public override Expression<Func<IDbItem, long>> GetSelector(FilterArgs args)
        {
            if(!args.TargetsDB)
                return a => (a as SaveAuction).FlatenedNBT.Where(n => n.Key == PropName || n.Key == "dungeon_item_level").Select(n => long.Parse(n.Value)).FirstOrDefault();
            var key = NBT.Instance.GetKeyId(PropName);
            var key2 = NBT.Instance.GetKeyId("dungeon_item_level");
            return a => a.NBTLookup.Where(l => l.KeyId == key || l.KeyId == key2).OrderByDescending(l=>l.KeyId == key2 ? 1 : 0).Select(l => l.Value).FirstOrDefault();
        }
    }
}

