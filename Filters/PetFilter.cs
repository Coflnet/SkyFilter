using System;
using hypixel;

namespace Coflnet.Sky.Filter
{
    public abstract class PetFilter : GeneralFilter
    {
        public override Func<DBItem, bool> IsApplicable => item => (item?.Tag?.StartsWith("PET_")  ?? false) && !(item?.Tag?.StartsWith("PET_ITEM") ?? true);
    }
}
