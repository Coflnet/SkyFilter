namespace Coflnet.Sky.Filter;
[FilterDescription("Abicase model for historic data, the item is now split up into multiple items")]
public class ModelFilter : NBTFilter
{
    protected override string PropName => "model";
}