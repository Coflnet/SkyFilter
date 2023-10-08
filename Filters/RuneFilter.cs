using System.Collections.Generic;

namespace Coflnet.Sky.Filter;
[FilterDescription("Applied rune (level) 0 for none")]
public abstract class RuneFilter : NBTNumberFilter
{
    public override IEnumerable<object> Options => new object[] { 0, 3 };
}
