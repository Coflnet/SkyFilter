using System.Linq;
using System.Collections.Generic;
using System;

namespace Coflnet.Sky.Filter
{
    [FilterDescription("Applied skin on an item")]
    public class SkinFilter : NBTItemFilter
    {
        protected override string PropName => "skin";
        public override Func<Items.Client.Model.Item, bool> IsApplicable => item => 
                !PetFilter.IsPet(item) // pet skin items are prefixed with pet skin
                && item.Modifiers.Any(m => m.Slug == PropName);

        public override IEnumerable<object> OptionsGet(OptionValues options)
        {
            // exclude http links and minecraft skin ids
            return options.Options[PropName].Where(o => !o.ToString().Contains("http") && o.ToString().Length < 60).Prepend(Any).Append(None);
        }
    }
}
