namespace Coflnet.Sky.Filter;

[FilterDescription("Which century the raffle item is from")]
public class RaffleYearFilter : NBTFilter
{
    protected override string PropName => "raffle_year";
    protected override long GetValueLong(string stringValue, short key)
    {
        return long.Parse(stringValue);
    }
}
