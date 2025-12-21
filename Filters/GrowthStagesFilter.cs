using System;
using System.Collections.Generic;

namespace Coflnet.Sky.Filter;

public class GrowthStagesFilter : NBTNumberFilter
{
    public override FilterType FilterType => FilterType.RANGE;
    public override IEnumerable<object> Options => [0,6];

    public override Func<Coflnet.Sky.Items.Client.Model.Item, bool> IsApplicable => item
                => item?.Tag == "ROSEWATER_FLASK";

    protected override string PropName => "seconds_held";

    public override long GetLowerBound(FilterArgs args, long input)
    {
        return SecondsForLevel(args, input);
    }

    public override long GetUpperBound(FilterArgs args, long input)
    {
        return SecondsForLevel(args, input +1);
    }

    /// <summary>
    /// Each level are 50 hours
    /// </summary>
    /// <param name="args"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    private static long SecondsForLevel(FilterArgs args, long input)
    {
        if (input <= 0)
            return 0;
        // add a small offset to match observed in-game boundaries
        return input * 50 * 60 * 60 + 2000;
    }
}
