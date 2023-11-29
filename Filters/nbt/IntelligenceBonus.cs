namespace Coflnet.Sky.Filter;

[FilterDescription("Bottle of Jyrre bonus")]
public class IntelligenceBonus : NBTNumberFilter
{
    protected override string PropName => "bottle_of_jyrre_seconds";
    public override long GetLowerBound(FilterArgs args, long input)
    {
        return input * 3600;
    }

    public override long GetUpperBound(FilterArgs args, long input)
    {
        return (input + 1) * 3600 - 1;
    }
}