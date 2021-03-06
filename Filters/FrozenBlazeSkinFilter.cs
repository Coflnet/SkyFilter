using System.Collections.Generic;
using Coflnet.Sky.Core;
using System;

namespace Coflnet.Sky.Filter
{
    public class FrozenBlazeSkinFilter : SkinFilter
    {
        public override IEnumerable<object> Options => new string[]{"FROZEN_BLAZE_ICICLE"};

        public override Func<Coflnet.Sky.Items.Client.Model.Item, bool> IsApplicable => item => item.Tag == "FROZEN_BLAZE_HELMET";
    }
}
