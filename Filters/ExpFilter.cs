using System;
using System.Collections.Generic;

namespace Coflnet.Sky.Filter;
[FilterDescription("Filter by (pet) experience")]
public class PetExpFilter : NBTNumberFilter
{
    public override IEnumerable<object> Options => [0, (long)int.MaxValue * 4];
    public override IEnumerable<object> OptionsGet(OptionValues options)
    {
        return Options;
    }
    public override Func<Items.Client.Model.Item, bool> IsApplicable => PetFilter.IsPet;
    protected override string PropName => "exp";
}
