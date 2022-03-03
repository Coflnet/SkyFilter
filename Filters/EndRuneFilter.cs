using System;
using hypixel;

namespace Coflnet.Sky.Filter
{
    public class EndRuneFilter : RuneFilter
    {
        public override Func<DBItem, bool> IsApplicable =>  a
            => a.Category == Category.WEAPON;
        protected override string PropName => "DRAGON";
    }
}

