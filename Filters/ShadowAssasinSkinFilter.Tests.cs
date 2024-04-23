using Coflnet.Sky.Core;
using NUnit.Framework;

namespace Coflnet.Sky.Filter;
public class ShadowAssasinSkinFilterTests
{
    [Test]
    public void Matches()
    {
        var filter = new ShadowAssasinSkinFilter();
        var item = new SaveAuction();
        item.Tag = "SHADOW_ASSASSIN_HELMET";
        var filters = new FilterArgs(new() { { "ShadowAssasinSkin", "none" } }, false);
        Assert.That(filter.GetExpression(filters).Compile()(item));
    }

    [Test]
    public void DoesntMatch()
    {
        var filter = new ShadowAssasinSkinFilter();
        var item = new SaveAuction();
        item.Tag = "SHADOW_ASSASSIN_HELMET";
        var filters = new FilterArgs(new() { { "ShadowAssasinSkin", NBTFilter.Any } }, false);
        Assert.That(!filter.GetExpression(filters).Compile()(item));
    }

    [Test]
    public void DoesntMatch2()
    {
        var filter = new ShadowAssasinSkinFilter();
        var item = new SaveAuction();
        item.Tag = "x";
        var filters = new FilterArgs(new() { { "ShadowAssasinSkin", "none" } }, false);
        Assert.That(!filter.GetExpression(filters).Compile()(item));
    }
}