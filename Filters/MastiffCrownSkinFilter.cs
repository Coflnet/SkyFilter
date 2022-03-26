using System.Collections.Generic;
using Coflnet.Sky.Core;
using System;

namespace Coflnet.Sky.Filter
{
    public class MastiffCrownSkinFilter : SkinFilter
    {
        public override IEnumerable<object> Options => new string[] { "MASTIFF_PUPPY" };

        public override Func<DBItem, bool> IsApplicable => item => item.Tag == "MASTIFF_HELMET";
    }
}
