using System;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter
{
    public class SpiderKillsFilter : NBTNumberFilter
    {
        public override Func<DBItem, bool> IsApplicable => a
            => a.Tag.StartsWith("TARANTULA_") || a.Tag.StartsWith("RECLUSE_");
        protected override string PropName => "spider_kills";
    }
}

