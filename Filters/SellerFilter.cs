using System.Collections.Generic;
using System.Linq;
using hypixel;

namespace Coflnet.Sky.Filter
{
    /// <summary>
    /// Filters for a seller by either username or minecraft uuid
    /// </summary>
    public class SellerFilter : GeneralFilter
    {
        public override FilterType FilterType => FilterType.Equal;
        public override IEnumerable<object> Options => new object[] { "Technoblade" };

        public override IQueryable<SaveAuction> AddQuery(IQueryable<SaveAuction> query, FilterArgs args)
        {
            var key = NBT.GetLookupKey("uid");
            var playerId = args.Get(this);

            var player = PlayerService.Instance.GetPlayer(playerId).Result;
            if(player == null)
                throw new CoflnetException("unkown_player",$"The player `{playerId}` was not found");

            return query.Where(a => a.SellerId == 0 ? a.AuctioneerId == player.UuId : a.SellerId == player.Id);
        }
    }
}
