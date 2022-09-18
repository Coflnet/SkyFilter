using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter
{
    /// <summary>
    /// Filters for a seller by either username or minecraft uuid
    /// </summary>
    public class SellerFilter : GeneralFilter
    {
        public override FilterType FilterType => FilterType.TEXT;
        public override IEnumerable<object> Options => new object[] { "" };

        public override Expression<System.Func<SaveAuction, bool>> GetExpression(FilterArgs args)
        {
            var playerId = args.Get(this);
            if(string.IsNullOrEmpty(playerId))
                return a => true;
            if(playerId.Length == 32 && !args.TargetsDB)
                return a => a.AuctioneerId == playerId;
            var player = PlayerService.Instance.GetPlayer(playerId).Result;
            if(player == null)
                throw new CoflnetException("unkown_player",$"The player `{playerId}` was not found");

            return a => a.SellerId == 0 ? a.AuctioneerId == player.UuId : a.SellerId == player.Id;
        }
    }
}
