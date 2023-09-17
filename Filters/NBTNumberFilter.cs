using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Coflnet.Sky.Core;

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

        public override Func<Coflnet.Sky.Items.Client.Model.Item, bool> IsApplicable => a
            => a.Modifiers.Any(m => m.Slug == PropName);

        public override IEnumerable<object> OptionsGet(OptionValues options)
        {
            if (!options.Options.TryGetValue(PropName, out List<string> values))
                throw new CoflnetException("no_options", "There are no options for " + PropName);
            var all = values.Where(o => double.TryParse(o, out _)).Select(o => double.Parse(o)).ToList();
            yield return 0; // none
            if (all.Min() != all.Max() || all.Max() < 10)
                yield return all.Max();
        }

        public override Expression<Func<IDbItem, long>> GetSelector(FilterArgs args)
        {
            var key = NBT.Instance.GetKeyId(PropName);
            if (!args.TargetsDB)
                return a => (a as SaveAuction).FlatenedNBT.Where(n => n.Key == PropName).Select(n => (long)double.Parse(n.Value)).FirstOrDefault();
            return a => a.NBTLookup.Where(l => l.KeyId == key).Select(l => l.Value).FirstOrDefault();
        }
    }
}

