namespace Coflnet.Sky.Filter;

[FilterDescription("Changes based on dungeon floor level obtained")]
public class ItemTierFilter : NBTNumberFilter
{
    protected override string PropName => "item_tier";
}