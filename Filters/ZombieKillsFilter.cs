using System;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter
{
    public class ZombieKillsFilter : NBTNumberFilter
    {
        protected override string PropName => "zombie_kills";
    }
}

