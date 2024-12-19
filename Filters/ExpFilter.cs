using System;

namespace Coflnet.Sky.Filter;
[FilterDescription("Filter by (pet) experience")]
public class PetExpFilter : NBTNumberFilter
{
    public override Func<Items.Client.Model.Item, bool> IsApplicable => PetFilter.IsPet;
    protected override string PropName => "exp";
}
