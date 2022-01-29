using System;
using System.Collections.Generic;
using hypixel;

namespace Coflnet.Sky.Filter
{
    public class FilterArgs
    {
        public Dictionary<string, string> Filters { get; }
        public bool TargetsDB { get; }

        public FilterArgs(Dictionary<string, string> filters, bool targetsDB)
        {
            Filters = filters;
            TargetsDB = targetsDB;
        }

        public DateTime GetAsTimeStamp(IFilter filter)
        {
            return GetAsLong(filter).ThisIsNowATimeStamp();
        }
        public long GetAsLong(IFilter filter)
        {
            if (long.TryParse(Get(filter), out long val))
                return val;
            throw new CoflnetException("invalid_number", "The passed filter has to be a number");
        }

        public string Get(IFilter filter)
        {
            if (Filters.TryGetValue(filter.Name, out string value))
                return value;
            throw new CoflnetException("missing_filter", $"The filter `{filter.Name}` is required for another filter");
        }
    }
}
