using System;
using System.Collections.Generic;
using System.Linq;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter
{
    public abstract class RuneFilter : NBTNumberFilter
    {

        public override IEnumerable<object> Options => new object[]{1,3};
    }
}

