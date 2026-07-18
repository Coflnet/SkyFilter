using System;
using System.Linq.Expressions;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter;

[FilterDescription("Whether the item has a creation time set")]
public class HasCreationTimeFilter : BoolFilter
{
    public override Expression<Func<IDbItem, bool>> GetBool(FilterArgs args)
    {
        return a => (a as SaveAuction).ItemCreatedAt > DateTime.MinValue;
    }
}
