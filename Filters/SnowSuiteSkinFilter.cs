using System.Collections.Generic;
using Coflnet.Sky.Core;
using System;

namespace Coflnet.Sky.Filter;
public class SnowSuiteSkinFilter : SkinFilter
{
    public override Func<Coflnet.Sky.Items.Client.Model.Item, bool> IsApplicable => item => item.Tag == "SNOW_SUIT_HELMET";

    protected override Func<SaveAuction, bool> ItemCheck()
    {
        return a => a.Tag == "SNOW_SUIT_HELMET";
    }
}
