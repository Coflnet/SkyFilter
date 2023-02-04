using NUnit.Framework;
using Coflnet.Sky.Core;
using System.Linq;

namespace Coflnet.Sky.Filter;

public class PricePerLevelFilterTests
{
    FilterArgs args;
    private SaveAuction sampleAuction;
    private PricePerLevelFilter filter;

    [SetUp]
    public void Setup()
    {
        filter = new PricePerLevelFilter();
        args = new FilterArgs(new System.Collections.Generic.Dictionary<string, string>() { { "sharpness", ">5" }, { "PricePerLevel", "10-20" } }, false, new FilterEngine());
        sampleAuction = new Core.SaveAuction()
        {
            ItemName = "[Lvl 33] TestPet",
            StartingBid = 400,
            Enchantments = new System.Collections.Generic.List<Enchantment>() { new Enchantment() { Type = Enchantment.EnchantmentType.sharpness, Level = 6 } }
        };
        NBT.Instance = new MockNbt();
    }
    /*[Test]
    public void Test()
    {
        var selector = filter.GetExpression(args);
        var value = selector.Compile().Invoke(sampleAuction);
        Assert.IsTrue(value);
    }*/
    [TestCase("<1m", 2, 1900000, true)]
    [TestCase("<1m", 2, 2000100, false)]
    [TestCase("<1m", 1, 900000, true)]
    [TestCase("<1m", 3, 3999000, true)]
    [TestCase("<1m", 4, 7999000, true)]
    [TestCase("<1m", 5, 15999000, true)]
    [TestCase("<1m", 7, 63999000, true)]
    [TestCase("<1m", 7, 64099000, false)]
    [Test]
    public void Lvl2(string selector, byte level, long startingBid, bool expected)
    {
        args.Filters["PricePerLevel"] = selector;
        sampleAuction.Enchantments.First().Level = level;
        sampleAuction.StartingBid = startingBid;
        var expression = filter.GetExpression(args);
        System.Console.WriteLine(expression);
        var value = expression.Compile().Invoke(sampleAuction);
        Assert.AreEqual(expected, value);
    }
    [TestCase("<1m", 2, 1900000, true)]
    public void SwitchedInput(string selector, byte level, long startingBid, bool expected)
    {
        args = new FilterArgs(new() { { "PricePerLevel", selector }, { "sharpness", ">5" } }, false, new FilterEngine());
        sampleAuction.Enchantments.First().Level = level;
        sampleAuction.StartingBid = startingBid;
        var expression = filter.GetExpression(args);
        System.Console.WriteLine(expression);
        var value = expression.Compile().Invoke(sampleAuction);
        Assert.AreEqual(expected, value);
    }
}