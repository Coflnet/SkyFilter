namespace Coflnet.Sky.Filter;
[FilterDescription("Year of the party hat, some hats may be different items")]
public class PartyHatYearFilter : NBTNumberFilter
{
    public override FilterType FilterType => FilterType.Equal;
    protected override string PropName => "party_hat_year";
}

