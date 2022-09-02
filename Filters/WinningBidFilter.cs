using System;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter
{
    public class WinningBidFilter : NBTNumberFilter
    {
        protected override string PropName => "winning_bid";
    }
}

