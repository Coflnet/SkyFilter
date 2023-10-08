using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter;
[FilterDescription("Stack item count")]
public class CountFilter : NumberFilter
{
    private string PropName => "count";
    public override Func<Items.Client.Model.Item, bool> IsApplicable => a
            => a.Modifiers.Any(m => m.Slug == PropName && m.Value.Count() > 1);
    public override IEnumerable<object> OptionsGet(OptionValues options)
    {
        return options.Options.GetValueOrDefault(PropName, new List<string>());
    }
    public override Expression<Func<IDbItem, long>> GetSelector(FilterArgs args)
    {
        return a => a.Count;
    }
}
