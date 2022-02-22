using System;
using System.Linq;
using System.Linq.Expressions;
using hypixel;

namespace Coflnet.Sky.Filter
{

    /// <summary>
    /// SELECT Tag FROM `NBTLookups`, Auctions a
    /// where AuctionId = a.Id
    /// and KeyId = (select id from `NBTKeys` where Slug = "spider_kills")
    /// and a.Id > 40000000;
    /// </summary>
    public abstract class NBTNumberFilter : NumberFilter
    {
        protected abstract string PropName { get; }
        public override Expression<Func<SaveAuction, long>> GetSelector(FilterArgs args)
        {
            var key = NBT.Instance.GetKeyId(PropName);
            return a => a.NBTLookup.Where(l => l.KeyId == key).Select(l => l.Value).FirstOrDefault();
        }
    }
}

