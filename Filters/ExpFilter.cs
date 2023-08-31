namespace Coflnet.Sky.Filter;
[FilterDescription("Filter by (pet) experience")]
public class ExpFilter : NBTNumberFilter
{
    protected override string PropName => "exp";
}
