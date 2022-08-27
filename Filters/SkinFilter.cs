using System.Linq;
using Microsoft.EntityFrameworkCore;
using Coflnet.Sky.Core;
using System.Collections.Generic;
using System.Linq.Expressions;
using System;
using Coflnet.Sky.Items.Client.Model;

namespace Coflnet.Sky.Filter
{
    public class SkinFilter : GeneralFilter
    {
        public override FilterType FilterType => FilterType.Equal;
        protected string PropName => "skin";

        public override Func<Item, bool> IsApplicable => a
            => a.Modifiers.Any(m => m.Slug == PropName);


        public override IEnumerable<object> OptionsGet(OptionValues options)
        {
            return options.Options[PropName].Where(o => !o.ToString().Contains("http") && o.ToString().Length != 64);
        }

        public override Expression<System.Func<SaveAuction, bool>> GetExpression(FilterArgs args)
        {
            var item = ItemDetails.Instance.GetItemIdForTag(args.Get(this));
            var key = NBT.Instance.GetKeyId("skin");
            return a => a.NBTLookup.Where(l => l.KeyId == key && l.Value == item).Any();
        }
    }
}
