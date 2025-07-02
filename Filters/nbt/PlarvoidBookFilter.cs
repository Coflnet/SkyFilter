namespace Coflnet.Sky.Filter;

[FilterDescription("Amount of polarvoid books")]
public class PlarvoidBookFilter : NBTNumberFilter
{
    protected override string PropName => "polarvoid";
}