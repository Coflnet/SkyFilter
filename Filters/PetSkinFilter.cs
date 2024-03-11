using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Coflnet.Sky.Core;
using System.Linq.Expressions;
using System;

namespace Coflnet.Sky.Filter
{
    [FilterDescription("This filter restricts applied skins to just pets. For more skin name options use the skin filter.")]
    public class PetSkinFilter : SkinFilter
    {
        public override Func<Items.Client.Model.Item, bool> IsApplicable => item => false; // don't display on items anymore

        public override IEnumerable<object> OptionsGet(OptionValues options)
        {
            // exclude http links and minecraft skin ids
            return base.OptionsGet(options).Where(o => o.ToString().StartsWith("PET")).Prepend(Any).Append(None);
        }
        public override Expression<Func<IDbItem, bool>> GetExpression(FilterArgs args)
        {
            if (args.Get(this).Equals(Any, StringComparison.OrdinalIgnoreCase) || string.IsNullOrEmpty(args.Get(this)))
            {
                if (args.TargetsDB)
                    return a => a.NBTLookup.Where(l => l.KeyId == NBT.Instance.GetKeyId("skin")).Any() && EF.Functions.Like(a.Tag, $"PET_%");
                return a => (a as SaveAuction).FlatenedNBT.ContainsKey("skin") && a.Tag.StartsWith("PET_");
            }
            if (args.Get(this).Equals(None, StringComparison.OrdinalIgnoreCase))
            {
                if (args.TargetsDB)
                    return a => !a.NBTLookup.Where(l => l.KeyId == NBT.Instance.GetKeyId("skin")).Any() && EF.Functions.Like(a.Tag, $"PET_%");
                return a => !(a as SaveAuction).FlatenedNBT.ContainsKey("skin") && a.Tag.StartsWith("PET_");
            }
            if (!args.TargetsDB)
                return a => (a as SaveAuction).FlatenedNBT.GetValueOrDefault("skin") == args.Get(this).Replace("PET_SKIN_", "") && a.Tag.StartsWith("PET_");
            var item = ItemDetails.Instance.GetItemIdForTag("PET_SKIN_" + args.Get(this));
            return a => a.NBTLookup.Where(l => l.KeyId == NBT.Instance.GetKeyId("skin") && l.Value == item).Any() && EF.Functions.Like(a.Tag, $"PET_%");
        }
    }
}
