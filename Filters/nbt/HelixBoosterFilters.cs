using System;
using System.Linq;
using System.Linq.Expressions;
using Coflnet.Sky.Core;
using Item = Coflnet.Sky.Items.Client.Model.Item;

namespace Coflnet.Sky.Filter;

/// <summary>
/// Booster tiers of Helix (Galatea) gear. The indexer flattens <c>booster_tiers.*</c> to bare leaf keys
/// like <c>sweep</c> or <c>fighting</c>, so the filters are limited to HELIX_ items to stay unambiguous.
/// </summary>
public abstract class HelixBoosterFilter : NBTNumberFilter
{
    public const string HelixTagPrefix = "HELIX_";

    public override Func<Item, bool> IsApplicable => item
        => item?.Tag?.StartsWith(HelixTagPrefix, StringComparison.Ordinal) == true
            && item.Modifiers?.Any(m => m.Slug == PropName) == true;

    /// <summary>
    /// The leaf keys are not unique across items, so matching is restricted to HELIX_ items as well
    /// </summary>
    public override Expression<Func<IDbItem, bool>> GetExpression(FilterArgs args)
        => base.GetExpression(args).And(a => a.Tag != null && a.Tag.StartsWith(HelixTagPrefix));
}

[FilterDescription("Foraging fortune booster tier 1-2 on Helix gear, None for no booster (0 means not present, supports number ranges)")]
public class HelixForagingFortuneFilter : HelixBoosterFilter
{
    protected override string PropName => "foraging_fortune";
}

[FilterDescription("Foraging wisdom booster tier 1-2 on Helix gear, None for no booster (0 means not present, supports number ranges)")]
public class HelixForagingWisdomFilter : HelixBoosterFilter
{
    protected override string PropName => "foraging_wisdom";
}

[FilterDescription("Sweep booster tier 1-2 on Helix gear, None for no booster (0 means not present, supports number ranges)")]
public class HelixSweepFilter : HelixBoosterFilter
{
    protected override string PropName => "sweep";
}

[FilterDescription("Hunting wisdom booster tier 1-2 on Helix gear, None for no booster (0 means not present, supports number ranges)")]
public class HelixHuntingWisdomFilter : HelixBoosterFilter
{
    protected override string PropName => "hunting_wisdom";
}

[FilterDescription("Fighting booster tier 1-2 on Helix gear, None for no booster (0 means not present, supports number ranges)")]
public class HelixFightingFilter : HelixBoosterFilter
{
    protected override string PropName => "fighting";
}
