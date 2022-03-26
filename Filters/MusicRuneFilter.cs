using System;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter
{
    public class MusicRuneFilter : RuneFilter
    {
        public override Func<DBItem, bool> IsApplicable =>  a
            => a.Category == Category.WEAPON;
        protected override string PropName => "MUSIC";
    }
}

