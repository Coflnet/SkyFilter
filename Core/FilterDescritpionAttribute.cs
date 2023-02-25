using System;

namespace Coflnet.Sky.Filter
{
    public class FilterDescritpionAttribute : Attribute
    {
        public string Description { get; set; }
        public bool Disabled { get; set; }
        public FilterDescritpionAttribute(string description, bool disabled = false)
        {
            Description = description;
            Disabled = disabled;
        }
    }
}
