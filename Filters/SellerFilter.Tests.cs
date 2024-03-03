using System.Collections.Generic;
using Coflnet.Sky.Core;
using NUnit.Framework;

namespace Coflnet.Sky.Filter;

public class SellerFilterTests
{
    [Test]
    public void Matches()
    {
        var filter = new SellerFilter();
        var args = new FilterArgs(new Dictionary<string, string>() { { "Seller", "384a029294fc445e863f2c42fe9709cb" }, { "forceBlacklist", "true" } }, false);
        var expression = filter.GetExpression(args);
        var result = expression.Compile().Invoke(new SaveAuction() { AuctioneerId = "384a029294fc445e863f2c42fe9709cb" });
        Assert.IsTrue(result);
    }
}