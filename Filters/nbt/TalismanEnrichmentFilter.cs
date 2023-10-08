namespace Coflnet.Sky.Filter;

[FilterDescription("Enrichment of the talisman, from community shop")]
public class TalismanEnrichmentFilter : NBTFilter
{
    protected override string PropName => "talisman_enrichment";
}