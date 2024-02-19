using System;
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
        return a => !(a as SaveAuction).Enchantments.Where(e => !otherEnchants.Contains(e.Type) && e.Level >= 6).Any();
    }
}