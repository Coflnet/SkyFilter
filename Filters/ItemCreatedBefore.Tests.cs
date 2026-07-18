using System;
using System.Collections.Generic;
using Coflnet.Sky.Core;
using NUnit.Framework;

namespace Coflnet.Sky.Filter;

public class ItemCreatedBeforeFilterTests
{
    // 2025-01-01 UTC in unix seconds
    private const long Threshold = 1735689600;

    [Test]
    public void MatchesItemCreatedBeforeThreshold()
    {
        var compiled = Compile();
        Assert.That(compiled(MakeAuction(new DateTime(2020, 1, 1))), Is.True);
    }

    [Test]
    public void DoesNotMatchItemCreatedAfterThreshold()
    {
        var compiled = Compile();
        Assert.That(compiled(MakeAuction(new DateTime(2030, 1, 1))), Is.False);
    }

    [Test]
    public void DoesNotMatchItemWithoutCreationDate()
    {
        var compiled = Compile();
        // ItemCreatedAt defaults to DateTime.MinValue when no creation date is set
        Assert.That(compiled(MakeAuction(default)), Is.False);
    }

    private static Func<IDbItem, bool> Compile()
    {
        var filter = new ItemCreatedBeforeFilter();
        var args = new FilterArgs(new Dictionary<string, string>
        {
            { "ItemCreatedBefore", Threshold.ToString() }
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
