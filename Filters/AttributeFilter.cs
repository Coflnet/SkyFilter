using System;
using System.Collections.Generic;

namespace Coflnet.Sky.Filter
{
    /// <summary>
    /// Generic filter for attributes
    /// </summary>
    public class AttributeFilter : NBTNumberFilter
    {
        private string _propName;
        private string _filterName;
        private int min;
        private int max;
        protected override string PropName => _propName;
        public override IEnumerable<object> Options => new object[] { min, max };

        public AttributeFilter(string propName, int min = 0, int max = Int32.MaxValue, string filterName = null)
        {
            _propName = propName;
            this.min = min;
            this.max = max;
            if (filterName != null)
                _filterName = filterName;
            else
                _filterName = propName;
        }

        public override string Name => _filterName;
    }
}

