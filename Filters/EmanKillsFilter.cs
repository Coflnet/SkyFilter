using System;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter
{
    public class EmanKillsFilter : NBTNumberFilter
    {
        public override Func<DBItem, bool> IsApplicable => a
            => a.Tag.StartsWith("FINAL_DESTINATION_");
        protected override string PropName => "eman_kills";
    }
}

