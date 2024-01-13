using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter;
[FilterDescription("Applied rune (level) 0 for none")]
public abstract class RuneFilter : NBTNumberFilter
{
    public override IEnumerable<object> Options => new object[] { 0, 3 };

    public override Expression<Func<IDbItem, long>> GetSelector(FilterArgs args)
    {
        if (!args.TargetsDB)
            return a => (a as SaveAuction).FlatenedNBT.Where(n => n.Key == PropName || n.Key == "RUNE_" + PropName).Select(n => (long)double.Parse(n.Value)).FirstOrDefault();
        var key = NBT.Instance.GetKeyId(PropName);
        var secondKey = NBT.Instance.GetKeyId("RUNE_" + PropName);
        return a => a.NBTLookup.Where(l => l.KeyId == key || l.KeyId == secondKey).Select(l => l.Value).FirstOrDefault();
    }
}
