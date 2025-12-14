namespace Coflnet.Sky.Filter;

[FilterDescription("How many ultimate chimera were found with this bookshelf")]
public class ChimeraFoundFilter : NBTNumberFilter
{
    protected override string PropName => "chimera_found";
}

