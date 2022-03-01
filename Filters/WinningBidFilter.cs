using System;
using hypixel;

namespace Coflnet.Sky.Filter
{
    public class WinningBidFilter : NBTNumberFilter
    {
        public override Func<DBItem, bool> IsApplicable => a
            => a.Tag == "MIDAS_STAFF" || a.Tag == "MIDAS_SWORD";
        protected override string PropName => "winning_bid";
    }
}

