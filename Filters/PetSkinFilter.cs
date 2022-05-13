using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Coflnet.Sky.Core;
using System.Linq.Expressions;

namespace Coflnet.Sky.Filter
{
    public class PetSkinFilter : PetFilter
    {
        public override FilterType FilterType => FilterType.Equal;

        public override IEnumerable<object> Options => ItemDetails.Instance.TagLookup.Keys.Where(k => k.StartsWith("PET_SKIN")).Prepend("any");

        public override Expression<System.Func<SaveAuction, bool>> GetExpression(FilterArgs args)
        {
            var key = NBT.Instance.GetKeyId("skin");
            if (args.Get(this) == "any" || string.IsNullOrEmpty(args.Get(this)))
            {
                if (args.TargetsDB)
                    return a => a.NBTLookup.Where(l => l.KeyId == key).Any();
                return a => a.FlatenedNBT.ContainsKey("skin");
            }
            var item = ItemDetails.Instance.GetItemIdForTag(args.Get(this));
            if (args.TargetsDB)
                return a => a.NBTLookup.Where(l => l.KeyId == key && l.Value == item).Any();
            return a => a.FlatenedNBT.GetValueOrDefault("skin") == args.Get(this);
        }
    }
}
