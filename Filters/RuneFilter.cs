using System;
using System.Collections.Generic;
using hypixel;

namespace Coflnet.Sky.Filter
{
    public abstract class RuneFilter : NBTNumberFilter
    {
        public override Func<DBItem, bool> IsApplicable =>  a
            => a.Category == Category.WEAPON || a.Category == Category.ARMOR;

        public override IEnumerable<object> Options => new object[]{1,3};
    }
}

