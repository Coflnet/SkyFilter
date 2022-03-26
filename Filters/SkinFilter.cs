using System.Linq;
using Microsoft.EntityFrameworkCore;
using Coflnet.Sky.Core;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Coflnet.Sky.Filter
{
    public abstract class SkinFilter : GeneralFilter
    {
        public override FilterType FilterType => FilterType.Equal;

        public override Expression<System.Func<SaveAuction, bool>> GetExpression(FilterArgs args)
        {
            var item = ItemDetails.Instance.GetItemIdForName(args.Get(this));
            var key = NBT.Instance.GetKeyId("skin");
            return a => a.NBTLookup.Where(l => l.KeyId == key && l.Value == item).Any();
        }
    }
}
