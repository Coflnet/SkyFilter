namespace Coflnet.Sky.Filter;

[FilterDescription("Art of the War applied or not")]
public class ArtOfTheWarFilter : BoolNbtFilter
{
    public override string Key => "art_of_war_count";
}
