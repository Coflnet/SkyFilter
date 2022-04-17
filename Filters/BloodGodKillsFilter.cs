using System;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter
{
    public class BloodGodKillsFilter : NBTNumberFilter
    {
        public override Func<DBItem, bool> IsApplicable => a
            => a.Tag == "BLOOD_GOD_CREST";
        protected override string PropName => "blood_god_kills";
    }
}

