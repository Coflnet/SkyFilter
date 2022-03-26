using System;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter
{
    public class ExpertiseKillsFilter : NBTNumberFilter
    {
        public override Func<DBItem, bool> IsApplicable => a
            => a.Tag.StartsWith("ROD_OF") || a.Tag.EndsWith("_ROD") || a.Tag == "THE_SHREDDER";
        protected override string PropName => "expertise_kills";
    }
}

