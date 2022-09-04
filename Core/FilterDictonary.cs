using System;
using System.Collections.Generic;

namespace Coflnet.Sky.Filter
{
    public class FilterDictonary : Dictionary<string,IFilter>
    {
        public void Add<TFilter>()  where TFilter : IFilter
        {
            this.Add(Activator.CreateInstance<TFilter>());
        }

        public void Add(IFilter filter)
        {
            this.Add(filter.Name,filter);
        }
    }
}
