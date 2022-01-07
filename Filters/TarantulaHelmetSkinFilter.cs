using System.Collections.Generic;
using hypixel;
using System;

namespace Coflnet.Sky.Filter
{
    public class TarantulaHelmetSkinFilter : SkinFilter
    {
        public override IEnumerable<object> Options => new string[]{"TARANTULA_BLACK_WIDOW"};

        public override Func<DBItem, bool> IsApplicable => item => item.Tag == "TARANTULA_HELMET";
    }
}
