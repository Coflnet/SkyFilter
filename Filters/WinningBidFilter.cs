using System;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter
{
    [FilterDescription("The highest bid on dark auction items")]
    public class WinningBidFilter : NBTNumberFilter
    {
        protected override string PropName => "winning_bid";
    }
}

