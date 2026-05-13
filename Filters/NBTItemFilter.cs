using System;
using System.Linq;
using System.Linq.Expressions;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter
{
    public abstract class NBTItemFilter : NBTFilter
    {
        protected override long GetValueLong(string stringValue, short key, FilterArgs args)
        {
            return args.Engine.itemDetails.GetItemIdForTag(stringValue.ToUpper());
        }

        protected override bool MatchesNoDbValue(string actualValue, string filterValue)
        {
            if (actualValue == null || filterValue == null)
                return actualValue == filterValue;

            return string.Equals(actualValue, filterValue, StringComparison.OrdinalIgnoreCase);
        }
    }
}

