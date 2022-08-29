using System.Collections.Generic;
using Coflnet.Sky.Core;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Coflnet.Sky.Filter
{
    public class PerfectGemsCountFilter : NumberFilter
    {
        HashSet<long> perfectValues = new();
        HashSet<short> perfectKeys = new();

        public virtual void LoadOptions()
        {
            if (perfectValues.Count > 0)
                return;

            using (var db = new HypixelContext())
            {
                var perfects = db.NBTValues.Where(v => v.Value == "PERFECT").ToHashSet();
                perfectKeys = perfects.Select(p =>p.KeyId).ToHashSet();
                perfectValues = perfects.Select(p =>(long) p.Id).ToHashSet();

            }
        }

        public override Func<Coflnet.Sky.Items.Client.Model.Item, bool> IsApplicable => i => i.Modifiers.Any(m => m.Value == "PERFECT");

        public override IEnumerable<object> Options => new string[] { "0", "5" };

        public override Expression<Func<SaveAuction, long>> GetSelector(FilterArgs args)
        {
            LoadOptions();
            if (!args.TargetsDB)
                return a => a.FlatenedNBT.Count(f => f.Value == "PERFECT");
            return a => a.NBTLookup.Where(l => perfectKeys.Contains(l.KeyId) && perfectValues.Contains(l.Value)).Count();
        }
    }
}
