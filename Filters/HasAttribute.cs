using System;
using System.Linq;
using System.Linq.Expressions;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter;
[FilterDescription("Any attribute (crimson isle) present")]
public class HasAttribute : BoolFilter
{
    public override Func<Coflnet.Sky.Items.Client.Model.Item, bool> IsApplicable => a
        => a.Modifiers.Any(m => FilterEngine.AttributeKeys.Contains(m.Slug));


    public override Expression<Func<IDbItem, bool>> GetBool(FilterArgs args)
    {
        if (!args.TargetsDB)
            return a => (a as SaveAuction).FlatenedNBT.Any(m => FilterEngine.AttributeKeys.Contains(m.Key));
        var keys = FilterEngine.AttributeKeys.Select(k => args.NbtIntance.GetKeyId(k)).ToArray();
        return a => a.NBTLookup.Any(l => keys.Contains(l.KeyId));
    }
}
