using System;
using System.Collections.Generic;
using System.Linq;
using MessagePack;

namespace Coflnet.Sky.Filter
{
    [MessagePackObject]
    public class FilterOptions
    {
        [Key("name")]
        public string Name;
        [Key("options")]
        public IEnumerable<string> Options;
        [Key("type")]
        public FilterType Type;
        [Key("longType")]
        public string LongType;

        public FilterOptions()
        {
        }

        public FilterOptions(IFilter filter, Dictionary<string, List<string>> all)
        {
            Name = filter.Name;
            Options = filter.OptionsGet(new OptionValues(all)).Select(o => o?.ToString());
            Type = filter.FilterType;
            LongType = Type.ToString();
        }
    }
}
