using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter
{
    public class CapturedPlayerFilter : GeneralFilter
    {
        public override FilterType FilterType => FilterType.Equal | FilterType.TEXT | FilterType.PLAYER_WITH_RANK;
        public override IEnumerable<object> Options => new object[] { "", "" };

        public override Func<Coflnet.Sky.Items.Client.Model.Item, bool> IsApplicable => item
                    => item?.Tag == "CAKE_SOUL";

        public override Expression<Func<IDbItem, bool>> GetExpression(FilterArgs args)
        {
            var key = NBT.Instance.GetKeyId("captured_player");
            var name = args.Get(this);
            if(string.IsNullOrWhiteSpace(name))
                return a => !a.NBTLookup.Where(l => l.KeyId == key).Any();
            var val = NBT.Instance.GetValueId(key,name);
            return a => a.NBTLookup.Where(l => l.KeyId == key && l.Value == val).Any() || a.ItemName == name;
        }
    }
}
