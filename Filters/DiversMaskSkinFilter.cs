using System.Collections.Generic;
using Coflnet.Sky.Core;
using System;

namespace Coflnet.Sky.Filter
{
    public class DiversMaskSkinFilter : SkinFilter
    {
        public override IEnumerable<object> Options => new string[] { "DIVER_PUFFER" };

        public override Func<Coflnet.Sky.Items.Client.Model.Item, bool> IsApplicable => item => item.Tag == "DIVER_HELMET";
    }
}
