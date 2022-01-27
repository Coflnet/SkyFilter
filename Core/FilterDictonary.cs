using System;
using System.Collections.Generic;

namespace Coflnet.Sky.Filter
{
    public class FilterDictonary : Dictionary<string,IFilter>
    {
        public void Add<TFilter>()  where TFilter : IFilter
        {
            var filter = Activator.CreateInstance<TFilter>();
            this.Add(filter.Name,filter);
        }
    }
}
