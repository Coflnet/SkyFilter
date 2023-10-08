using System;
using System.Linq;

namespace Coflnet.Sky.Filter;
[FilterDescription("Was wood singularity applied")]
public class WoodSingularityFilter : BoolNbtFilter
{
    public override Func<Coflnet.Sky.Items.Client.Model.Item, bool> IsApplicable => item
                => new string[] { "TACTICIAN_SWORD", "SWORD_OF_REVELATIONS", "WOOD_SWORD" }.Contains(item?.Tag);

    public override string Key => "wood_singularity_count";

}
