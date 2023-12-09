using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter
{
    public class GeneralRuneFilter : RuneFilter
    {
        private string _propName;
        private string _filterName;
        protected override string PropName => _propName;
        public override string Name => _filterName;

        public GeneralRuneFilter(string propName, string filterName)
        {
            _propName = propName;
            _filterName = filterName;
        }
    }
}

