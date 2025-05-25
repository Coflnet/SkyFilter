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


    public override Func<Coflnet.Sky.Items.Client.Model.Item, bool> IsApplicable => a
        => a.Modifiers.Any(m => m.Slug == PropName.Replace("RUNE_","") || m.Slug == PropName);

    public override Expression<Func<IDbItem, long>> GetSelector(FilterArgs args)
    {
        var secondProp = PropName.Replace("RUNE_","");
        if (!args.TargetsDB)
            return a => (a as SaveAuction).FlatenedNBT.Where(n => n.Key == PropName || n.Key == secondProp).Select(n => (long)double.Parse(n.Value)).FirstOrDefault();
        var key = args.NbtIntance.GetKeyId(PropName);
        var secondKey = args.NbtIntance.GetKeyId(PropName.Replace("RUNE_",""));
        return a => a.NBTLookup.Where(l => l.KeyId == key || l.KeyId == secondKey).Select(l => l.Value).FirstOrDefault();
    }
}
