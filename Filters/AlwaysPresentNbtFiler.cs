using System;
using System.Collections.Generic;
using System.Linq;

namespace Coflnet.Sky.Filter;

public abstract class AlwaysPresentNbtFiler : NBTFilter
{
    public override Func<Items.Client.Model.Item, bool> IsApplicable => a
        => a.Modifiers.Where(m => m.Slug == PropName).Count() > 1;
    public override IEnumerable<object> OptionsGet(OptionValues options)
    {
        return options.Options.GetValueOrDefault(PropName, new List<string>()).Where(a => !int.TryParse(a, out int _));
    }
}