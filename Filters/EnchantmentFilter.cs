using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Coflnet.Sky.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Coflnet.Sky.Filter
{
    public class EnchantmentFilter : GeneralFilter
    {
        public override FilterType FilterType => FilterType.Equal;
        public override Func<Coflnet.Sky.Items.Client.Model.Item, bool> IsApplicable =>
                EnchantLvlFilter.IsEnchantable();
        public override IEnumerable<object> Options => Enum.GetNames(typeof(Enchantment.EnchantmentType)).OrderBy(e => e);

        protected virtual string EnchantLvlName { get; } = "EnchantLvlFilter";

        public override Expression<Func<IDbItem, bool>> GetExpression(FilterArgs args)
        {
            if (!Enum.TryParse<Enchantment.EnchantmentType>(args.Get(this), true, out Enchantment.EnchantmentType enchant))
                throw new CoflnetException("invalid_filter", $"The value `{args.Get(this)}` is not a known enchant");
            if (enchant == Enchantment.EnchantmentType.None)
                return a => (a as SaveAuction).Enchantments == null || (a as SaveAuction).Enchantments.Count == 0;
            if (!args.Filters.ContainsKey(EnchantLvlName))
                return a => (a as SaveAuction).Enchantments.Where(e => e.Type == enchant).Any();
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
            return item => false; // deprecated filter item.Modifiers.Any(m => m.Slug.StartsWith("!enc"));
        }

        public virtual string EnchantmentKey { get; set; } = "Enchantment";

        public override Expression<Func<IDbItem, bool>> GetExpression(FilterArgs args)
        {
            if (!args.Filters.ContainsKey(EnchantmentKey))
                throw new CoflnetException("invalid_filter", "You need to select an enchantment and a lvl to filter for");
            var enchant = GetEnchant(args);
            var filterValue = args.Get(this);
            if (!short.TryParse(args.Get(this), out short lvl))
                return base.GetExpression(args);
            if (!args.Filters.ContainsKey("ItemId"))
                if (args.TargetsDB)
                    return a => (a as SaveAuction).Enchantments != null && (a as SaveAuction).Enchantments.Where(e => e.Type == enchant && e.Level == lvl && e.SaveAuctionId >= MinimumAuctionId).Any();
                else
                    return a => (a as SaveAuction).Enchantments != null && (a as SaveAuction).Enchantments.Where(e => e.Type == enchant && e.Level == lvl).Any();
            var itemid = int.Parse(args.Filters["ItemId"]);
            if (args.TargetsDB)
                return a => (a as SaveAuction).Enchantments != null && (a as SaveAuction).Enchantments.Where(e => itemid == e.ItemType && e.Type == enchant && e.Level == lvl && e.SaveAuctionId >= MinimumAuctionId).Any();
            return a => (a as SaveAuction).Enchantments != null && (a as SaveAuction).Enchantments.Where(e => itemid == e.ItemType && e.Type == enchant && e.Level == lvl).Any();
        }

        private Enchantment.EnchantmentType GetEnchant(FilterArgs args)
        {
            if (!Enum.TryParse(args.Filters[EnchantmentKey], true, out Enchantment.EnchantmentType enchant))
                throw new CoflnetException("invalid_filter", $"The value `{args.Filters[EnchantmentKey]}` is not a known enchant");
            return enchant;
        }

        public override Expression<Func<IDbItem, long>> GetSelector(FilterArgs args)
        {
            var enchant = Enum.Parse<Enchantment.EnchantmentType>(args.Filters[EnchantmentKey], true);
            if (enchant == Enchantment.EnchantmentType.None)
                return a => 1;
            return a => (a as SaveAuction).Enchantments.Where(e => e.Type == enchant).Select(e => (int)e.Level).FirstOrDefault();
        }

        public override async Task LoadData(IServiceProvider provider)
        {
            using var scope = provider.CreateScope();
            try
            {
                using var db = scope.ServiceProvider.GetService<HypixelContext>();
                MinimumAuctionId = await db.Auctions.MaxAsync(a => a.Id) - 30000000;
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to load data for enchantment filter, db probably not available");
            }
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
