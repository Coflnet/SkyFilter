using System;
using hypixel;

namespace Coflnet.Sky.Filter
{
    public class MusicRuneFilter : RuneFilter
    {
        public override Func<DBItem, bool> IsApplicable =>  a
            => a.Category == Category.WEAPON;
        protected override string PropName => "MUSIC";
    }
}

