namespace Coflnet.Sky.Filter;

[FilterDescription("Blaze kills")]
public class BlazeConsumerFilter : NBTNumberFilter
{
    protected override string PropName => "blaze_consumer";
}