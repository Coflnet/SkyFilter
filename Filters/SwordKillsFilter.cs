using System;
using hypixel;

namespace Coflnet.Sky.Filter
{
    public class SwordKillsFilter : NBTNumberFilter
    {
        public override Func<DBItem, bool> IsApplicable => a
            => a.Tag == "FEL_SWORD";
        protected override string PropName => "sword_kills";
    }
}

