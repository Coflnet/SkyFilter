using System.Collections.Generic;
using Coflnet.Sky.Core;
using System;

namespace Coflnet.Sky.Filter
{
    public class TarantulaHelmetSkinFilter : SkinFilter
    {
        public override IEnumerable<object> Options => new string[]{"TARANTULA_BLACK_WIDOW"};

        public override Func<Coflnet.Sky.Items.Client.Model.Item, bool> IsApplicable => item => item.Tag == "TARANTULA_HELMET";
    }
}
