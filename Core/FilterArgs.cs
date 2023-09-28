using System;
using System.Collections.Generic;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter
{
    public class FilterArgs
    {
        public Dictionary<string, string> Filters { get; }
        public bool TargetsDB { get; }
        public FilterEngine Engine { get; set; }

        public FilterArgs(Dictionary<string, string> filters, bool targetsDB, FilterEngine engine = null)
        {
            Filters = filters;
            TargetsDB = targetsDB;
            Engine = engine;
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
        public double GetAsDouble(IFilter filter)
        {
            if (double.TryParse(Get(filter), out double val))
                return val;
            throw new CoflnetException("invalid_number", "The passed filter has to be a number");
        }

        public string Get(IFilter filter)
        {
            return Get(filter.Name);
        }
        public string Get(string filterName)
        {
            if (TryGet(filterName, out string value))
                return value;
            throw new CoflnetException("missing_filter", $"The filter `{filterName}` is required for another filter");
        }
        public bool TryGet(IFilter filter, out string value)
        {
            return TryGet(filter.Name, out value);
        }
        public bool TryGet(string filterName, out string value)
        {
            return Filters.TryGetValue(filterName, out value);
        }
    }
}
