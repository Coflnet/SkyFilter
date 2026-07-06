using System.Collections.Generic;
using Coflnet.Sky.Core;
using NUnit.Framework;

namespace Coflnet.Sky.Filter;

public class CakeSoulColorFilterTests
{
    [Test]
    public void MatchesColorNameByDurability()
    {
        var filter = new CakeSoulColorFilter();
        var args = new FilterArgs(new Dictionary<string, string>
        {
            { "CakeSoulColor", "Gray" }
        }, false, null);
        var compiled = filter.GetExpression(args).Compile();

        Assert.That(compiled(MakeAuction(8)), Is.True);
        Assert.That(compiled(MakeAuction(0)), Is.False);
        Assert.That(compiled(MakeAuction(15)), Is.False);
    }

    [Test]
    public void MatchesRawDurabilityNumber()
    {
        var filter = new CakeSoulColorFilter();
        var args = new FilterArgs(new Dictionary<string, string>
        {
            { "CakeSoulColor", "0" }
        }, false, null);
        var compiled = filter.GetExpression(args).Compile();

        Assert.That(compiled(MakeAuction(0)), Is.True); // Black
        Assert.That(compiled(MakeAuction(8)), Is.False);
    }

    private static SaveAuction MakeAuction(int durability)
    {
        return new SaveAuction
        {
            FlatenedNBT = new Dictionary<string, string>
            {
                { "soul_durability", durability.ToString() }
            }
        };
    }
}
