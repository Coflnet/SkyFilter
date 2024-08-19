namespace Coflnet.Sky.Filter;

[FilterDescription("Tuned transmission on aotv")]
public class TunedTransmissionFilter : NBTNumberFilter
{
    protected override string PropName => "tuned_transmission";
}