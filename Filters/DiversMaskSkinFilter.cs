using System.Collections.Generic;
using Coflnet.Sky.Core;
using System;

namespace Coflnet.Sky.Filter;
public class DiversMaskSkinFilter : SkinFilter
{
    public override Func<Coflnet.Sky.Items.Client.Model.Item, bool> IsApplicable => item => item.Tag == "DIVER_HELMET";

    protected override Func<IDbItem, bool> ItemCheck()
    {
        return a => a.Tag == "DIVER_HELMET";
    }
}
