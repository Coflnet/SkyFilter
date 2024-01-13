using System.Collections.Generic;

namespace Coflnet.Sky.Filter
{
    [FilterDescription("Is the item rarity upgraded with a recombobulator")]
    public class RecombobulatedFilter : BoolNbtFilter
    {
        public override FilterType FilterType => FilterType.BOOLEAN;

        public override IEnumerable<object> Options => new object[] { "true", "false" };

        public override string Key => "rarity_upgrades";
    }
}

