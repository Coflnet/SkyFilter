using System.Collections.Generic;
using System.Linq;
using Coflnet.Sky.Core;
using NUnit.Framework;

namespace Coflnet.Sky.Filter;

public class CakeYearFilterTests
{
    [Test]
    public void AcceptsCommaSeparatedList()
    {
        var filter = new CakeYearFilter();
        var args = new FilterArgs(new Dictionary<string, string>
        {
            { "CakeYear", "1, 3, 7" }
        }, false, null);
        var compiled = filter.GetExpression(args).Compile();

        Assert.That(compiled(MakeAuction(1)), Is.True);
        Assert.That(compiled(MakeAuction(3)), Is.True);
        Assert.That(compiled(MakeAuction(7)), Is.True);
        Assert.That(compiled(MakeAuction(2)), Is.False);
        Assert.That(compiled(MakeAuction(4)), Is.False);
    }

    [Test]
    public void AcceptsRangeAndCommaCombined()
    {
        var filter = new CakeYearFilter();
        var args = new FilterArgs(new Dictionary<string, string>
        {
            { "CakeYear", "1-2,5" }
        }, false, null);
        var compiled = filter.GetExpression(args).Compile();

        Assert.That(compiled(MakeAuction(1)), Is.True);
        Assert.That(compiled(MakeAuction(2)), Is.True);
        Assert.That(compiled(MakeAuction(5)), Is.True);
        Assert.That(compiled(MakeAuction(3)), Is.False);
        Assert.That(compiled(MakeAuction(6)), Is.False);
    }

    [Test]
    public void StillAcceptsSingleRange()
    {
        var filter = new CakeYearFilter();
        var args = new FilterArgs(new Dictionary<string, string>
        {
            { "CakeYear", "1-3" }
        }, false, null);
        var compiled = filter.GetExpression(args).Compile();

        Assert.That(compiled(MakeAuction(2)), Is.True);
        Assert.That(compiled(MakeAuction(4)), Is.False);
    }

    private static SaveAuction MakeAuction(int year)
    {
        return new SaveAuction
        {
            FlatenedNBT = new Dictionary<string, string>
            {
                { "new_years_cake", year.ToString() }
            }
        };
    }
}
