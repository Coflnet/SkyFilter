using System;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter
{
    public class TidalRuneFilter : RuneFilter
    {
        public override Func<DBItem, bool> IsApplicable =>  a
            => a.Category == Category.ARMOR;
        protected override string PropName => "TIDAL";
    }
}

