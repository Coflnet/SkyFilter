namespace Coflnet.Sky.Filter;

[FilterDescription("Captured soul count, supports ranges")]
public class NecromancerFilter : NBTNumberFilter
{
    protected override string PropName { get; }
    public override string Name => PropName;
    public NecromancerFilter(string attribute)
    {
        PropName = attribute;
    }
}

