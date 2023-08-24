using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter
{
    public class CakeYearFilter : NumberFilter
    {
        public override IEnumerable<object> Options => new object[] { 1, CurrentMinecraftYear() +1 };

        public override Func<Coflnet.Sky.Items.Client.Model.Item, bool> IsApplicable => item => item.Tag == "NEW_YEAR_CAKE";

        private static int CurrentMinecraftYear()
        {
            return (int)((DateTime.Now - new DateTime(2019, 6, 13)).TotalDays / (TimeSpan.FromDays(5) + TimeSpan.FromHours(4)).TotalDays + 1);
        }

        public override Expression<Func<IDbItem, long>> GetSelector(FilterArgs args)
        {
            var key = NBT.Instance.GetKeyId("new_years_cake");
            return a => a.NBTLookup.Where(l => l.KeyId == key).Select(l => l.Value).FirstOrDefault();
        }
    }
}

