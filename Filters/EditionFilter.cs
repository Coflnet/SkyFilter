using System;

namespace Coflnet.Sky.Filter
{
    public class EditionFilter : NBTNumberFilter
    {
        public override Func<DBItem, bool> IsApplicable => a
            => true;
        protected override string PropName => "edition";
    }
}

