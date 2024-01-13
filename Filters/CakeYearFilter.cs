using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter
{
    public class CakeYearFilter : NBTNumberFilter
    {
        public override IEnumerable<object> Options => new object[] { 1, CurrentMinecraftYear() +1 };

        protected override string PropName => "new_years_cake";

        private static int CurrentMinecraftYear()
        {
            return (int)((DateTime.Now - new DateTime(2019, 6, 13)).TotalDays / (TimeSpan.FromDays(5) + TimeSpan.FromHours(4)).TotalDays + 1);
        }
    }
}

