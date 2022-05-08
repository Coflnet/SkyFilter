using System;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter
{
    public class ZombieKillsFilter : NBTNumberFilter
    {
        public override Func<Coflnet.Sky.Items.Client.Model.Item, bool> IsApplicable => a
            => a.Tag.StartsWith("REVENANT_") || a.Tag.StartsWith("REAPER_");
        protected override string PropName => "zombie_kills";
    }
}

