using Coflnet.Sky.Core;
using NUnit.Framework;

namespace Coflnet.Sky.Filter;
public class NoOtherValuableEnchantsFilterTests
{
    [TestCase("sharpness", Enchantment.EnchantmentType.sharpness, 6, true)]
    [TestCase("sharpness", Enchantment.EnchantmentType.pristine, 5, false)]
    public void Matches(string filterName, Enchantment.EnchantmentType enchantmentType, byte level, bool expected)
    {
        var filter = new NoOtherValuableEnchantsFilter();
        var args = new FilterArgs(new() { { filterName, level.ToString() } }, true, null);
        var expr = filter.GetBool(args);
        var saveAuction = new SaveAuction()
        {
            Enchantments = new(){
                new Enchantment(enchantmentType, level)
            }
        };
        Assert.That(expr.Compile()(saveAuction), Is.EqualTo(expected));
    }

}