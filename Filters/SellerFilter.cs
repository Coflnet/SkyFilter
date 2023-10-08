using System.Collections.Generic;
using System.Linq.Expressions;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter;

/// <summary>
/// Filters for a seller by either username or minecraft uuid
/// </summary>
[FilterDescription("Seller uuid or username (slower)")]
public class SellerFilter : GeneralFilter
{
    public override FilterType FilterType => FilterType.TEXT;
    public override IEnumerable<object> Options => new object[] { "" };

    public override Expression<System.Func<IDbItem, bool>> GetExpression(FilterArgs args)
    {
        var playerId = args.Get(this);
        if (string.IsNullOrEmpty(playerId))
            return a => true;
        if (playerId.Length == 32 && !args.TargetsDB)
            return a => (a as SaveAuction).AuctioneerId == playerId;
        var player = PlayerService.Instance.GetPlayer(playerId).Result;
        if (player == null)
            throw new CoflnetException("unkown_player", $"The player `{playerId}` was not found");

        return a => (a as SaveAuction).SellerId == 0 ? (a as SaveAuction).AuctioneerId == player.UuId : (a as SaveAuction).SellerId == player.Id;
    }
}
