using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Coflnet.Sky.Core;
using System.Linq.Expressions;
using System;

namespace Coflnet.Sky.Filter
{
    public class PetSkinFilter : SkinFilter
    {
        public override Func<Coflnet.Sky.Items.Client.Model.Item, bool> IsApplicable => item => PetFilter.IsPet(item) && item.Modifiers.Any(m => m.Slug == PropName);

        public override Expression<Func<IDbItem, bool>> GetExpression(FilterArgs args)
        {
            var key = NBT.Instance.GetKeyId("skin");
            if (args.Get(this) == Any || string.IsNullOrEmpty(args.Get(this)))
            {
                if (args.TargetsDB)
                    return a => a.NBTLookup.Where(l => l.KeyId == key).Any() && EF.Functions.Like(a.Tag, $"PET_%");
                return a => (a as SaveAuction).FlatenedNBT.ContainsKey("skin") && a.Tag.StartsWith("PET_");
            }
            if (args.Get(this) == None)
            {
                if (args.TargetsDB)
                    return a => !a.NBTLookup.Where(l => l.KeyId == key).Any() && EF.Functions.Like(a.Tag, $"PET_%");
                return a => !(a as SaveAuction).FlatenedNBT.ContainsKey("skin") && a.Tag.StartsWith("PET_");
            }
            if (!args.TargetsDB)
                return a => (a as SaveAuction).FlatenedNBT.GetValueOrDefault("skin") == args.Get(this).Replace("PET_SKIN_", "") && a.Tag.StartsWith("PET_");
            var item = ItemDetails.Instance.GetItemIdForTag("PET_SKIN_" + args.Get(this));
            return a => a.NBTLookup.Where(l => l.KeyId == key && l.Value == item).Any() && EF.Functions.Like(a.Tag, $"PET_%");
        }
    }
}
