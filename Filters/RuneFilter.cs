using System.Collections.Generic;

namespace Coflnet.Sky.Filter
{
    public abstract class RuneFilter : NBTNumberFilter
    {
        public override IEnumerable<object> Options => new object[]{1,3};
    }
}

