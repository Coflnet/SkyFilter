namespace Coflnet.Sky.Filter;
[FilterDescription("Allows selecting a comma separated list of hex colors")]
public class HexColorListFilter : ColorFilter
{
    public override FilterType FilterType => FilterType.RANGE;
}
