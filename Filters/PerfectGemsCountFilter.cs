using System.Collections.Generic;
using Coflnet.Sky.Core;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Coflnet.Sky.Filter
{
    [FilterDescription("How many perfect gems are on the item")]
    public class PerfectGemsCountFilter : NumberFilter
    {
        HashSet<long> perfectValues = new();
        HashSet<short> perfectKeys = new();

        protected virtual string PropertyValueName => "PERFECT"; 

        public virtual void LoadOptions()
        {
            if (perfectValues.Count > 0)
                return;

            using (var db = new HypixelContext())
            {
                var perfects = db.NBTValues.Where(v => v.Value == PropertyValueName).ToHashSet();
                perfectKeys = perfects.Select(p =>p.KeyId).ToHashSet();
                perfectValues = perfects.Select(p =>(long) p.Id).ToHashSet();

            }
        }

        public override Func<Coflnet.Sky.Items.Client.Model.Item, bool> IsApplicable => i => i.Modifiers.Any(m => m.Value == PropertyValueName);

        public override IEnumerable<object> Options => new string[] { "0", "5" };

        public override Expression<Func<SaveAuction, long>> GetSelector(FilterArgs args)
        {
            LoadOptions();
            if (!args.TargetsDB)
                return a => a.FlatenedNBT.Count(f => f.Value == PropertyValueName);
            return a => a.NBTLookup.Where(l => perfectKeys.Contains(l.KeyId) && perfectValues.Contains(l.Value)).Count();
        }
    }

    [FilterDescription("How many flawless gems are on the item")]
    public class FlawlessGemsCountFilter : PerfectGemsCountFilter
    {

        protected override string PropertyValueName => "FLAWLESS";
    }
}
