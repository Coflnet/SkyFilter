using System;
using System.Collections.Generic;

namespace Coflnet.Sky.Filter
{
    public class FilterDictonary : Dictionary<string,IFilter>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Coflnet.Sky.Filter.FilterDictonary"/> class.
        /// Tells the base class to compare the keys case insensitive
        /// </summary>
        public FilterDictonary() : base(StringComparer.OrdinalIgnoreCase)
        {
        }
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
