using System.Collections.Generic;
using System.Linq;
using Coflnet.Sky.Core;
using NUnit.Framework;

namespace Coflnet.Sky.Filter;

public class CostPerExpFilterTests
{
    FilterArgs args;
    private SaveAuction sampleAuction;
    private CostPerExpPlusBaseFilter filter;
    [SetUp]
    public void Setup()
    {
        filter = new CostPerExpPlusBaseFilter();
        args = new FilterArgs(new System.Collections.Generic.Dictionary<string, string>() {  }, true, new FilterEngine(new MockNbt()));
        sampleAuction = new Core.SaveAuction()
        {
            ItemName = "[Lvl 33] TestPet",
            StartingBid = 400,
            NBTLookup = new List<NBTLookup>() { new () { KeyId = 2, Value = 5 } }
        };
    }
    
    [TestCase("1+0", 10, 10, true)]
    [TestCase("0.4-0.5+0", 10, 20, true)]
    [TestCase(">1+0", 20, 10, true)]
    [TestCase(">2+5", 20, 10, true)]
    [TestCase(">2+0", 20, 10, false)]
    [Test]
    public void Lvl2(string selector, byte level, long startingBid, bool expected)
    {
        args.Filters["CostPerExpPlusBase"] = selector;
        sampleAuction.NBTLookup.First().Value = level;
        sampleAuction.StartingBid = startingBid;
        var expression = filter.GetExpression(args);
        System.Console.WriteLine(expression);
        var val = filter.GetSelector(args).Compile().Invoke(sampleAuction);
        System.Console.WriteLine($"Expected value: {val}");
        var value = expression.Compile().Invoke(sampleAuction);
        Assert.That(expected,Is.EqualTo(value));
    }
}