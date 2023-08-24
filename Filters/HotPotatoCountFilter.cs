using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Coflnet.Sky.Core;
using Coflnet.Sky.Items.Client.Model;

namespace Coflnet.Sky.Filter
{
    public class HotPotatoCountFilter : GeneralFilter
    {
        public override FilterType FilterType => FilterType.Equal | FilterType.SIMPLE;

        public override IEnumerable<object> Options => new object[] { "none", "1-9", "10", "15" };

        public override Func<Items.Client.Model.Item, bool> IsApplicable => item
            => (item?.Category == ItemCategory.SWORD)
            || item.Category == ItemCategory.HELMET
            || item.Category == ItemCategory.CHESTPLATE
            || item.Category == ItemCategory.LEGGINGS
            || item.Category == ItemCategory.BOOTS
            || item.Category == ItemCategory.AXE
            || item.Category == ItemCategory.BOW
            || item.Category == ItemCategory.WAND;


        public override Expression<Func<IDbItem, bool>> GetExpression(FilterArgs args)
        {
            var key = NBT.Instance.GetKeyId("hpc");
            if (args.Get(this) == "none")
                return a => !a.NBTLookup.Where(l => l.KeyId == key).Any();
            if (args.Get(this) == "1-9")
                return a => !a.NBTLookup.Where(l => l.KeyId == key && l.Value > 0 && l.Value < 10).Any();
            var val = args.GetAsLong(this);
            return a => a.NBTLookup.Where(l => l.KeyId == key && l.Value == val).Any();
        }
    }
}

