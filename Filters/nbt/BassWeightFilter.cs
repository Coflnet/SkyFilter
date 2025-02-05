namespace Coflnet.Sky.Filter;

[FilterDescription("How high the bass weight metric is")]
public class BassWeightFilter : NBTNumberFilter
{
    protected override string PropName => "bass_weight";
}