using System;

namespace Coflnet.Sky.Filter
{
    public class FilterDescriptionAttribute : Attribute
    {
        public string Description { get; set; }
        public bool Disabled { get; set; }
        public FilterDescriptionAttribute(string description, bool disabled = false)
        {
            Description = description;
            Disabled = disabled;
        }
    }
}
