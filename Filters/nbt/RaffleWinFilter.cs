namespace Coflnet.Sky.Filter;

[FilterDescription("Which raffle roll the item is from")]
public class RaffleWinFilter : NBTFilter
{
    protected override string PropName => "raffle_win";
}