namespace Coflnet.Sky.Filter;

[FilterDescription("What boosters were applied to the axe")]
public class AxeBoostersFilter :NBTFilter
{
    protected override string PropName => "boosters";
}