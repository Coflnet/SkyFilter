using System.Collections.Generic;
using Coflnet.Sky.Core;
using System;

namespace Coflnet.Sky.Filter
{
    public class ReaperMaskSkinFilter : SkinFilter
    {
        public override IEnumerable<object> Options => new string[] { "REAPER_SPIRIT" };

        public override Func<Coflnet.Sky.Items.Client.Model.Item, bool> IsApplicable => item => item.Tag == "REAPER_MASK";
    }
}
