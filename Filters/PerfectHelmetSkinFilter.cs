using System.Collections.Generic;
using Coflnet.Sky.Core;
using System;

namespace Coflnet.Sky.Filter
{
    public class PerfectHelmetSkinFilter : SkinFilter
    {
        public override IEnumerable<object> Options => new string[] { "PERFECT_FORGE" };

        public override Func<Coflnet.Sky.Items.Client.Model.Item, bool> IsApplicable => item => item.Tag.StartsWith("PERFECT_HELMET");
    }
}
