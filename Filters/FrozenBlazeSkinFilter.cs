using System.Collections.Generic;
using Coflnet.Sky.Core;
using System;

namespace Coflnet.Sky.Filter;
public class FrozenBlazeSkinFilter : SkinFilter
{
    public override Func<Coflnet.Sky.Items.Client.Model.Item, bool> IsApplicable => item => item.Tag == "FROZEN_BLAZE_HELMET";

    protected override Func<IDbItem, bool> ItemCheck()
    {
        return a => a.Tag == "FROZEN_BLAZE_HELMET";
    }
}
