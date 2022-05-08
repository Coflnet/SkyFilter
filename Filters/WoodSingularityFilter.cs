using System;
using System.Linq;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter
{

    public class WoodSingularityFilter : BoolNbtFilter
    {
        public override Func<Coflnet.Sky.Items.Client.Model.Item, bool> IsApplicable => item
                    => new string[] { "TACTICIAN_SWORD", "SWORD_OF_REVELATIONS", "WOOD_SWORD" }.Contains(item?.Tag);

        public override string Key => "wood_singularity_count";

    }
}
