using System;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter
{
    public class SwordKillsFilter : NBTNumberFilter
    {
        public override Func<DBItem, bool> IsApplicable => a
            => a.Tag == "FEL_SWORD";
        protected override string PropName => "sword_kills";
    }
}

