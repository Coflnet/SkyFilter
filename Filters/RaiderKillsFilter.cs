using System;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter
{
    public class RaiderKillsFilter : NBTNumberFilter
    {
        protected override string PropName => "raider_kills";
    }
}

