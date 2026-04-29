using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter
{
    [FilterDescription("Supports number ranges (e.g. 1-3), comma separated lists (e.g. 1,3,7) and combinations")]
    public class CakeYearFilter : NBTNumberFilter
    {
        public override IEnumerable<object> Options => new object[] { 1, CurrentMinecraftYear() + 1 };

        protected override string PropName => "new_years_cake";

        public override IEnumerable<object> OptionsGet(OptionValues options)
        {
            return Options;
        }

        protected override string GetValue(FilterArgs args)
        {
            var value = args.Get(this);
            if (string.IsNullOrEmpty(value))
                return value;
            // Allow comma separated lists in addition to the pipe separator that NumberFilter understands.
            // Whitespace around the separators is also tolerated so users can write "1, 3, 7".
            return value.Replace(", ", ",").Replace(" ,", ",").Replace(',', '|');
        }

        private static int CurrentMinecraftYear()
        {
            return (int)((DateTime.Now - new DateTime(2019, 6, 13)).TotalDays / (TimeSpan.FromDays(5) + TimeSpan.FromHours(4)).TotalDays + 1);
        }
    }
}

