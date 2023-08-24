using System.Collections.Generic;
using Coflnet.Sky.Core;
using System;

namespace Coflnet.Sky.Filter
{
    public class ReaperMaskSkinFilter : SkinFilter
    {
        public override Func<Coflnet.Sky.Items.Client.Model.Item, bool> IsApplicable => item => item.Tag == "REAPER_MASK";

        protected override Func<IDbItem, bool> ItemCheck()
        {
            return a => a.Tag == "REAPER_MASK";
        }
    }
}
