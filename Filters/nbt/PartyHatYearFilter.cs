namespace Coflnet.Sky.Filter
{
    public class PartyHatYearFilter : NBTNumberFilter
    {
        public override FilterType FilterType => FilterType.Equal;
        protected override string PropName => "party_hat_year";
    }
}

