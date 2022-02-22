using System;
using hypixel;

namespace Coflnet.Sky.Filter
{
    public class ZombieKillsFilter : NBTNumberFilter
    {
        public override Func<DBItem, bool> IsApplicable => a
            => a.Tag.StartsWith("REVENANT_") || a.Tag.StartsWith("REAPER_");
        protected override string PropName => "zombie_kills";
    }
}

