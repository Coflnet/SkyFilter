namespace Coflnet.Sky.Filter;

[FilterDescription("Coins collected on Crown of Avarice")]
public class CollectedCoinsFilter : NBTFilter
{
    protected override string PropName => "collected_coins";
}