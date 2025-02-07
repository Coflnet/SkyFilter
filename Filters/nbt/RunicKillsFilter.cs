namespace Coflnet.Sky.Filter;

[FilterDescription("How many runic mobs were killed")]
public class RunicKillsFilter : NBTNumberFilter
{
    protected override string PropName => "runic_kills";
}