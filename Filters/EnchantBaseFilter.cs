using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Coflnet.Sky.Core;
using Newtonsoft.Json;

namespace Coflnet.Sky.Filter
{
    public class EnchantBaseFilter : NumberFilter
    {
        private Enchantment.EnchantmentType enchant;
        public override string Name => enchant.ToString();

        public EnchantBaseFilter(Enchantment.EnchantmentType enchant)
        {
            this.enchant = enchant;
        }

        public override IEnumerable<object> OptionsGet(OptionValues options)
        {
            var all = options.Options.Where(o => o.Key.StartsWith("!ench"+enchant)).FirstOrDefault().Value;
            if(all == null)
                return new object[] { 0 };
            var ints = all.Select(o => Convert.ToInt32(o));
            return new object[]{ints.Min(), ints.Max()};
        }

        public override Func<Coflnet.Sky.Items.Client.Model.Item, bool> IsApplicable =>
                i => i.Modifiers.Where(m => m.Slug.StartsWith("!ench"+enchant)).Any();

        public override Expression<Func<SaveAuction, long>> GetSelector(FilterArgs args)
        {
            return a => a.Enchantments.Where(e => e.Type == enchant).Select(e => (int)e.Level).FirstOrDefault();
        }
    }
}
