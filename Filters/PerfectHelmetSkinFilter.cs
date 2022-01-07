using System.Collections.Generic;
using hypixel;
using System;

namespace Coflnet.Sky.Filter
{
    public class PerfectHelmetSkinFilter : SkinFilter
    {
        public override IEnumerable<object> Options => new string[] { "PERFECT_FORGE" };

        public override Func<DBItem, bool> IsApplicable => item => item.Tag.StartsWith("PERFECT_HELMET");
    }
}
