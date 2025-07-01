namespace Coflnet.Sky.Filter;

[FilterDescription("How many logs were cut using the axe")]
public class LogsCutFilter : NBTNumberFilter
{
    protected override string PropName => "logs_cut";
}

