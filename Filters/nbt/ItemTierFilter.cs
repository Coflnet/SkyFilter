namespace Coflnet.Sky.Filter;

[FilterDescription("Changes based on dungeon floor level obtained")]
public class ItemTierFilter : NBTFilter
{
    protected override string PropName => "item_tier";
}