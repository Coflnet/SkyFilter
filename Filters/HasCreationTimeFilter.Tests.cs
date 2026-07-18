using System;
using System.Collections.Generic;
using Coflnet.Sky.Core;
using NUnit.Framework;

namespace Coflnet.Sky.Filter;

public class HasCreationTimeFilterTests
{
    [Test]
    public void TrueMatchesItemsWithCreationTime()
    {
        var compiled = Compile("true");
        Assert.That(compiled(MakeAuction(new DateTime(2020, 1, 1))), Is.True);
        Assert.That(compiled(MakeAuction(default)), Is.False);
    }

    [Test]
    public void FalseMatchesItemsWithoutCreationTime()
    {
        var compiled = Compile("false");
        Assert.That(compiled(MakeAuction(new DateTime(2020, 1, 1))), Is.False);
        Assert.That(compiled(MakeAuction(default)), Is.True);
    }

    private static Func<IDbItem, bool> Compile(string value)
    {
        var filter = new HasCreationTimeFilter();
        var args = new FilterArgs(new Dictionary<string, string>
        {
            { "HasCreationTime", value }
        }, false, null);
        return filter.GetExpression(args).Compile();
    }

    private static SaveAuction MakeAuction(DateTime createdAt)
    {
        return new SaveAuction
        {
            ItemCreatedAt = createdAt
        };
    }
}
