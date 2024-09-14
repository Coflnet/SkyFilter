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
        public override string Name { get; }

        public EnchantBaseFilter(Enchantment.EnchantmentType enchant, string name = null)
        {
            this.enchant = enchant;
            Name = name ?? enchant.ToString();
        }

        public override IEnumerable<object> OptionsGet(OptionValues options)
        {
            var all = options.Options.Where(o => o.Key.StartsWith("!ench" + enchant)).FirstOrDefault().Value;
            if (all == null)
                return new object[] { 0 };
            var ints = all.Select(o => Convert.ToInt32(o));
            return new object[] { 0, ints.Max() };
        }

        public override Func<Coflnet.Sky.Items.Client.Model.Item, bool> IsApplicable =>
                i => i.Modifiers.Where(m => m.Slug.StartsWith("!ench" + enchant)).Any();

        public override Expression<Func<IDbItem, long>> GetSelector(FilterArgs args)
        {
            if (args.TargetsDB)
                return a => (a as SaveAuction).Enchantments.Where(e => e.Type == enchant).Select(e => (int)e.Level).FirstOrDefault();
            return a => GetEnchantLevel(a);
        }

        private int GetEnchantLevel(IDbItem item)
        {
            if (item is not SaveAuction auction)
            {
                return 0;
            }
            foreach (var enchant in auction.Enchantments)
            {
                if (this.enchant == enchant.Type)
                {
                    return enchant.Level;
                }
            }
            return 0;
        }
    }
}
