using System;
using hypixel;

namespace Coflnet.Sky.Filter
{
    public class TidalRuneFilter : RuneFilter
    {
        public override Func<DBItem, bool> IsApplicable =>  a
            => a.Category == Category.ARMOR;
        protected override string PropName => "TIDAL";
    }
}

