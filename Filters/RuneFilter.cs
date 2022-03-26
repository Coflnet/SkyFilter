using System;
using System.Collections.Generic;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter
{
    public abstract class RuneFilter : NBTNumberFilter
    {
        public override Func<DBItem, bool> IsApplicable =>  a
            => a.Category == Category.WEAPON || a.Category == Category.ARMOR;

        public override IEnumerable<object> Options => new object[]{1,3};
    }
}

