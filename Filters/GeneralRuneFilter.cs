using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter
{
    public class GeneralRuneFilter : RuneFilter
    {
        private string _propName;
        protected override string PropName => _propName;
        public override string Name => ItemDetails.TagToName(_propName).Replace(" ", "") + "Rune";

        public GeneralRuneFilter(string propName)
        {
            _propName = propName;
        }
    }
}

