using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Coflnet.Sky.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Coflnet.Sky.Filter
{
    public class EnchantmentFilter : GeneralFilter
    {
        public override FilterType FilterType => FilterType.Equal;
        public override Func<Coflnet.Sky.Items.Client.Model.Item, bool> IsApplicable =>
                EnchantLvlFilter.IsEnchantable();
        public override IEnumerable<object> Options => Enum.GetNames(typeof(Enchantment.EnchantmentType)).OrderBy(e => e);

        protected virtual string EnchantLvlName { get; } = "EnchantLvlFilter";

        public override Expression<Func<SaveAuction, bool>> GetExpression(FilterArgs args)
        {
            var enchant = Enum.Parse<Enchantment.EnchantmentType>(args.Get(this), true);
            if (enchant == Enchantment.EnchantmentType.None)
                return a => a.Enchantments == null || a.Enchantments.Count == 0;
            if (!args.Filters.ContainsKey(EnchantLvlName))
                return a => a.Enchantments.Where(e => e.Type == enchant).Any();
            return null;
        }
    }



    public class EnchantLvlFilter : NumberFilter
    {
        private int MinimumAuctionId;
        public override IEnumerable<object> Options => new object[] { 1, 10 };
        public override Func<Coflnet.Sky.Items.Client.Model.Item, bool> IsApplicable =>
                IsEnchantable();

        public static Func<Coflnet.Sky.Items.Client.Model.Item, bool> IsEnchantable()
        {
            return item => item.Modifiers.Any(m => m.Slug.StartsWith("!enc"));
        }

        public virtual string EnchantmentKey { get; set; } = "Enchantment";

        public override Expression<Func<SaveAuction, bool>> GetExpression(FilterArgs args)
        {
            if (!args.Filters.ContainsKey(EnchantmentKey))
                throw new CoflnetException("invalid_filter", "You need to select an enchantment and a lvl to filter for");
            if (Enum.TryParse<Enchantment.EnchantmentType>(args.Filters[EnchantmentKey], true, out Enchantment.EnchantmentType enchant))
                throw new CoflnetException("invalid_filter", $"The value `{args.Filters[EnchantmentKey]}` is not a known enchant");;
            var filterValue = args.Get(this);
            if (!short.TryParse(args.Get(this), out short lvl))
                return base.GetExpression(args);
            if (!args.Filters.ContainsKey("ItemId"))
                if (args.TargetsDB)
                    return a => a.Enchantments != null && a.Enchantments.Where(e => e.Type == enchant && e.Level == lvl && e.SaveAuctionId >= MinimumAuctionId).Any();
                else
                    return a => a.Enchantments != null && a.Enchantments.Where(e => e.Type == enchant && e.Level == lvl).Any();
            var itemid = int.Parse(args.Filters["ItemId"]);
            if (args.TargetsDB)
                return a => a.Enchantments != null && a.Enchantments.Where(e => itemid == e.ItemType && e.Type == enchant && e.Level == lvl && e.SaveAuctionId >= MinimumAuctionId).Any();
            return a => a.Enchantments != null && a.Enchantments.Where(e => itemid == e.ItemType && e.Type == enchant && e.Level == lvl).Any();
        }

        public override Expression<Func<SaveAuction, long>> GetSelector(FilterArgs args)
        {
            var enchant = Enum.Parse<Enchantment.EnchantmentType>(args.Filters[EnchantmentKey], true);
            if (enchant == Enchantment.EnchantmentType.None)
                return a => 1;
            return a => a.Enchantments.Where(e => e.Type == enchant).Select(e => (int)e.Level).FirstOrDefault();
        }

        public override async Task LoadData(IServiceProvider provider)
        {
            using var db = provider.GetService<HypixelContext>();
            this.MinimumAuctionId = await db.Auctions.MaxAsync(a => a.Id) - 30000000;
        }
    }

    public class SecondEnchantmentFilter : EnchantmentFilter
    {
        protected override string EnchantLvlName => "SecondEnchantLvlFilter";
    }

    public class SecondEnchantLvlFilter : EnchantLvlFilter
    {
        public override string EnchantmentKey { get; set; } = "SecondEnchantment";
    }
}
