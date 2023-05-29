namespace Coflnet.Sky.Filter;
public class GemFilter : NBTFilter
{

    public override string Name => PropName.ToLower().Replace("_", "") + "Gem";

    protected override string PropName { get; }

    public GemFilter(string propName)
    {
        PropName = propName;
    }
}
