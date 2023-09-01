using System;
using System.Collections.Generic;

namespace Coflnet.Sky.Filter;
[FilterDescription("Filter by (pet) experience")]
public class PetExpFilter : NBTNumberFilter
{
    public override Func<Coflnet.Sky.Items.Client.Model.Item, bool> IsApplicable => PetFilter.IsPet;
    protected override string PropName => "exp";
    public override IEnumerable<object> OptionsGet(OptionValues options)
    {
        yield return 0;
        yield return PetLevelFilter.TotalMaxExp;
    }
}
