using System.Collections.Generic;
using System;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter;
public class PerfectHelmetSkinFilter : SkinFilter
{
    public override Func<Coflnet.Sky.Items.Client.Model.Item, bool> IsApplicable => item => item.Tag.StartsWith("PERFECT_HELMET");

    protected override Func<SaveAuction, bool> ItemCheck()
    {
        return a => a.Tag.StartsWith("PERFECT_HELMET");
    }
}
