using System.Collections.Generic;
using System.Linq;
using Coflnet.Sky.Core;
using NUnit.Framework;

namespace Coflnet.Sky.Filter;

public class CostPerExpFilterTests
{
    FilterArgs args;
    private SaveAuction sampleAuction;
    private CostPerExpFilter filter;
    [SetUp]
    public void Setup()
    {
        filter = new CostPerExpFilter();
        args = new FilterArgs(new System.Collections.Generic.Dictionary<string, string>() {  }, false, new FilterEngine());
        sampleAuction = new Core.SaveAuction()
        {
            ItemName = "[Lvl 33] TestPet",
            StartingBid = 400,
            NBTLookup = new List<NBTLookup>() { new () { KeyId = 2, Value = 5 } }
        };
        NBT.Instance = new MockNbt();
    }
    
    [TestCase("1", 10, 10, true)]
    [TestCase("0.4-0.5", 10, 20, true)]
    [TestCase(">1", 20, 10, true)]
    [Test]
    public void Lvl2(string selector, byte level, long startingBid, bool expected)
    {
        args.Filters["CostPerExp"] = selector;
        sampleAuction.NBTLookup.First().Value = level;
        sampleAuction.StartingBid = startingBid;
        var expression = filter.GetExpression(args);
        System.Console.WriteLine(expression);
        var val = filter.GetSelector(args).Compile().Invoke(sampleAuction);
        System.Console.WriteLine($"Expected value: {val}");
        var value = expression.Compile().Invoke(sampleAuction);
        Assert.AreEqual(expected, value);
    }
}