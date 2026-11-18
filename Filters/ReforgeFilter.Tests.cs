using System.Collections.Generic;
using Coflnet.Sky.Core;
using NUnit.Framework;

namespace Coflnet.Sky.Filter;

public class ReforgeFilterTests
{
    [TestCase("Mossy", ItemReferences.Reforge.mossy)]
    [TestCase("mossy", ItemReferences.Reforge.mossy)]
    [TestCase("Demonic", ItemReferences.Reforge.Demonic)]
    public void ReforgeValueIsMatchedIgnoringCase(string value, ItemReferences.Reforge reforge)
    {
        var expression = CreateExpression(value);

        Assert.That(expression(new SaveAuction { Reforge = reforge }), Is.True);
        Assert.That(expression(new SaveAuction { Reforge = ItemReferences.Reforge.None }), Is.False);
    }

    private static System.Func<IDbItem, bool> CreateExpression(string value)
    {
        var args = new FilterArgs(new Dictionary<string, string> { { "Reforge", value } }, false, new FilterEngine((INBT)null));
        return new ReforgeFilter().GetExpression(args).Compile();
    }
}
