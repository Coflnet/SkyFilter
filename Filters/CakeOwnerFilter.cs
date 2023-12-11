namespace Coflnet.Sky.Filter;

[FilterDescription("Cake owner name")]
public class CakeOwnerFilter : CapturedPlayerFilter
{
    protected override string Propname => "cake_owner";
}