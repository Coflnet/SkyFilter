namespace Coflnet.Sky.Filter;
[FilterDescription("Dye item that changed the color")]
public class DyeItemFilter : NBTItemFilter
{
    protected override string PropName => "dye_item";
}

