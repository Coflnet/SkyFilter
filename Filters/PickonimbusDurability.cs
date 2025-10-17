namespace Coflnet.Sky.Filter;

[FilterDescription("Selects Durability on Pickonimbus Pickaxes")]
public class PickonimbusDurabilityFilter : NBTNumberFilter
{
    protected override string PropName => "pickonimbus_durability";
}

