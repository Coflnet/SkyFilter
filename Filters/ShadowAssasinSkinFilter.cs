using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Coflnet.Sky.Core;
using System;
using System.Linq.Expressions;

namespace Coflnet.Sky.Filter
{
    public class ShadowAssasinSkinFilter : GeneralFilter
    {
        public override FilterType FilterType => FilterType.Equal;

        public override IEnumerable<object> Options => new string[] { "SHADOW_ASSASSIN_CRIMSON", "SHADOW_ASSASSIN_MAUVE", "SHADOW_ASSASSIN_ADMIRAL" };

        public override Func<Coflnet.Sky.Items.Client.Model.Item, bool> IsApplicable => item => item.Tag == "SHADOW_ASSASSIN_HELMET";

        public override IQueryable<SaveAuction> AddQuery(IQueryable<SaveAuction> query, FilterArgs args)
        {
            var item = ItemDetails.Instance.GetItemIdForTag(args.Get(this));
            var key = NBT.Instance.GetKeyId("skin");
            return query.Include(a => a.NBTLookup).Where(a => a.NBTLookup.Where(l => l.KeyId == key && l.Value == item).Any());
        }

        public override Expression<Func<SaveAuction, bool>> GetExpression(FilterArgs args)
        {
            var item = ItemDetails.Instance.GetItemIdForTag(args.Get(this));
            var key = NBT.Instance.GetKeyId("skin");
            return a => a.NBTLookup.Where(l => l.KeyId == key && l.Value == item).Any();
        }
    }
}
