using System;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter
{
    public class ZombieKillsFilter : NBTNumberFilter
    {
        public override Func<DBItem, bool> IsApplicable => a
            => a.Tag.StartsWith("REVENANT_") || a.Tag.StartsWith("REAPER_");
        protected override string PropName => "zombie_kills";
    }
}

