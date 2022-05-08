using System;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter
{
    public class WinningBidFilter : NBTNumberFilter
    {
        public override Func<Coflnet.Sky.Items.Client.Model.Item, bool> IsApplicable => a
            => a.Tag == "MIDAS_STAFF" || a.Tag == "MIDAS_SWORD";
        protected override string PropName => "winning_bid";
    }
}

