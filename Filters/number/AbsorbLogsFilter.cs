namespace Coflnet.Sky.Filter;

[FilterDescription("How many logs were absorbed (chopped) using an axe with enchantment")]
public class AbsorbLogsFilter : NBTNumberFilter
{
    protected override string PropName => "absorb_logs_chopped";
}
