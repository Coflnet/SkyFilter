using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using hypixel;
using Microsoft.EntityFrameworkCore;

namespace Coflnet.Sky.Filter
{
    public class EnchantmentFilter : GeneralFilter
    {
        public override FilterType FilterType => FilterType.Equal;
        public override Func<DBItem, bool> IsApplicable =>
                EnchantLvlFilter.IsEnchantable();
        public override IEnumerable<object> Options => Enum.GetNames(typeof(Enchantment.EnchantmentType)).OrderBy(e => e);
        public override IQueryable<SaveAuction> AddQuery(IQueryable<SaveAuction> query, FilterArgs args)
        {
            return query;
        }

        public override  Expression<Func<SaveAuction, bool>> GetExpression(FilterArgs args)
        {
            return null;
        }
    }

    

    public class EnchantLvlFilter : GeneralFilter
    {
        public override FilterType FilterType => FilterType.Equal | FilterType.SIMPLE;
        public override IEnumerable<object> Options => new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        public override Func<DBItem, bool> IsApplicable =>
                IsEnchantable();

        public static Func<DBItem, bool> IsEnchantable()
        {
            return item => item.Category == Category.WEAPON
                            || item.Category == Category.ARMOR
                            || item.Tag == "ENCHANTED_BOOK"
                            || item.Tag.Contains("_DRILL")
                            || item.Description.ToLower().Contains("axe") 
                            || item.Description.ToLower().Contains("shovel") 
                            || item.Description.ToLower().Contains("hoe");
        }

        public virtual string EnchantmentKey { get; set; } = "Enchantment";

        public override  Expression<Func<SaveAuction, bool>> GetExpression(FilterArgs args)
        {
            if (!args.Filters.ContainsKey(EnchantmentKey))
                throw new CoflnetException("invalid_filter", "You need to select an enchantment and a lvl to filter for");
            var enchant = Enum.Parse<Enchantment.EnchantmentType>(args.Filters[EnchantmentKey]);
            var lvl = (short)args.GetAsLong(this);
            if(!args.Filters.ContainsKey("ItemId"))
                return a => a.Enchantments != null && a.Enchantments.Where(e =>e.Type == enchant && e.Level == lvl).Any();
            var itemid = int.Parse(args.Filters["ItemId"]);
            return a => a.Enchantments != null &&  a.Enchantments.Where(e => itemid == e.ItemType && e.Type == enchant && e.Level == lvl).Any();
        }
    }

    public class SecondEnchantmentFilter : EnchantmentFilter
    {
        
    }

    public class SecondEnchantLvlFilter : EnchantLvlFilter
    {
        public override string EnchantmentKey { get; set; } = "SecondEnchantment";
    }
}
