using System.Collections.Generic;
using hypixel;
using System;

namespace Coflnet.Sky.Filter
{
    public class ReaperMaskSkinFilter : SkinFilter
    {
        public override IEnumerable<object> Options => new string[] { "REAPER_SPIRIT" };

        public override Func<DBItem, bool> IsApplicable => item => item.Tag == "REAPER_MASK";
    }
}
