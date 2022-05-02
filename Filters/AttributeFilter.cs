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
        private int min;
        private int max;
        protected override string PropName => _propName;
        public override IEnumerable<object> Options => new object[] { min, max };

        public AttributeFilter(string propName, int min = 0, int max = Int32.MaxValue)
        {
            _propName = propName;
            this.min = min;
            this.max = max;
        }

        public override string Name => _propName;
    }
}

