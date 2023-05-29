namespace Coflnet.Sky.Filter;
public class GemTypeFilter : NBTFilter
{
    public override string Name => prefix.ToLower().Replace("_", "") + "GemType";
    private string prefix;

    protected override string PropName => prefix + "_gem";

    public GemTypeFilter(string prefix)
    {
        this.prefix = prefix;
    }
}

