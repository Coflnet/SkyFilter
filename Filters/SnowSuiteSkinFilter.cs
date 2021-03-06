using System.Collections.Generic;
using Coflnet.Sky.Core;
using System;

namespace Coflnet.Sky.Filter
{
    public class SnowSuiteSkinFilter : SkinFilter
    {
        public override IEnumerable<object> Options => new string[]{"SNOW_SNOWGLOBE"};

        public override Func<Coflnet.Sky.Items.Client.Model.Item, bool> IsApplicable => item => item.Tag == "SNOW_SUIT_HELMET";
    }
}
