using System;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter
{
    public class EmanKillsFilter : NBTNumberFilter
    {
        protected override string PropName => "eman_kills";
    }
}

