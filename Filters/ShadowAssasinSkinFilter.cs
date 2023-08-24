using Coflnet.Sky.Core;
using System;

namespace Coflnet.Sky.Filter;
public class ShadowAssasinSkinFilter : SkinFilter
{
    public override Func<Coflnet.Sky.Items.Client.Model.Item, bool> IsApplicable => item => item.Tag == "SHADOW_ASSASSIN_HELMET";

    protected override Func<IDbItem, bool> ItemCheck()
    {
        return a => a.Tag == "SHADOW_ASSASSIN_HELMET";
    }
}