using System.Collections.Generic;
using System.Linq;
using Coflnet.Sky.Core;
using NUnit.Framework;

namespace Coflnet.Sky.Filter;

public class IntelligenceBonusTests
{
    FilterArgs args;
    private SaveAuction sampleAuction;
    private IntelligenceBonus filter;
    [SetUp]
    public void Setup()
    {
        filter = new ();
        args = new FilterArgs(new Dictionary<string, string>() {  }, true, new FilterEngine());
        sampleAuction = new SaveAuction()
        {
            StartingBid = 400,
            NBTLookup = new List<NBTLookup>() { new () { KeyId = 2, Value = 134550 } }
        };
        NBT.Instance = new MockNbt();
    }
    
    [TestCase("37", 134550, true)]
    [TestCase("0", 100, true)]
    [TestCase(">=42", 152270, true)]
    [TestCase(">42", 152270, false)]
    public void Lvl2(string selector, int seconds, bool expected)
    {
        args.Filters["IntelligenceBonus"] = selector;
        sampleAuction.NBTLookup.First().Value = seconds;
        var expression = filter.GetExpression(args);
        var val = filter.GetSelector(args).Compile().Invoke(sampleAuction);
        System.Console.WriteLine($"Expected value: {val}");
        System.Console.WriteLine(expression);
        var value = expression.Compile().Invoke(sampleAuction);
        Assert.That(expected,Is.EqualTo(value));
    }
}