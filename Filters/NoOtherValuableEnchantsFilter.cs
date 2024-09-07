using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter;
[FilterDescription("Besides enchants specified in filters all valuable enchants (lvl 6+) are not allowed")]
public class NoOtherValuableEnchantsFilter : BoolFilter
{
    public override Expression<Func<IDbItem, bool>> GetBool(FilterArgs args)
    {
        var allEnchants = Enum.GetValues(typeof(Enchantment.EnchantmentType)).Cast<Enchantment.EnchantmentType>().Where(e => e != Enchantment.EnchantmentType.None);
        var filternames = args.Filters.Keys;
        var otherEnchants = allEnchants.Where(e => filternames.Contains(e.ToString())).ToHashSet();
        if (!args.TargetsDB)
        {
            var levels = Constants.RelevantEnchants.Where(r => r.Level < 6 && !otherEnchants.Contains(r.Type)).ToDictionary(r => r.Type, r => r.Level);
            return a => !(a as SaveAuction).Enchantments.Where(e => !otherEnchants.Contains(e.Type) && e.Level >= 6 || levels.Where(o => o.Key == e.Type && o.Value <= e.Level).Any()).Any();
        }
        var level1List = new HashSet<Enchantment.EnchantmentType>();
        var level2List = new HashSet<Enchantment.EnchantmentType>();
        var level3List = new HashSet<Enchantment.EnchantmentType>();
        var level4List = new HashSet<Enchantment.EnchantmentType>();
        var level5List = new HashSet<Enchantment.EnchantmentType>();
        foreach(var enchant in Constants.RelevantEnchants)
        {
            if (otherEnchants.Contains(enchant.Type))
                continue;
            if (enchant.Level == 1)
                level1List.Add(enchant.Type);
            else if (enchant.Level == 2)
                level2List.Add(enchant.Type);
            else if (enchant.Level == 3)
                level3List.Add(enchant.Type);
            else if (enchant.Level == 4)
                level4List.Add(enchant.Type);
            else if (enchant.Level == 5)
                level5List.Add(enchant.Type);
        }
        return a => !(a as SaveAuction).Enchantments.Where(e => !otherEnchants.Contains(e.Type) && e.Level >= 6 
        || level1List.Contains(e.Type) && e.Level >= 1
        || level2List.Contains(e.Type) && e.Level >= 2
        || level3List.Contains(e.Type) && e.Level >= 3
        || level4List.Contains(e.Type) && e.Level >= 4
        || level5List.Contains(e.Type) && e.Level >= 5
        ).Any();
    }
}