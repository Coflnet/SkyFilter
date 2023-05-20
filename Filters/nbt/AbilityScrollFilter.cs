using System.Collections.Generic;
using System.Linq;

namespace Coflnet.Sky.Filter
{
    public class AbilityScrollFilter : NBTFilter
    {
        public override IEnumerable<object> OptionsGet(OptionValues options)
        {
            return options.Options.GetValueOrDefault(PropName, new List<string>()).Where(s=>!s.Contains("  ")).Append(None).Prepend(Any);
        }
        protected override string PropName => "ability_scroll";
    }
}
