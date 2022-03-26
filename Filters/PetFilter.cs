using System;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter
{
    public abstract class PetFilter : GeneralFilter
    {
        public override Func<DBItem, bool> IsApplicable => item => (item?.Tag?.StartsWith("PET_") ?? false)
        && !(item?.Tag?.StartsWith("PET_ITEM") ?? true)
        && !(item?.Tag?.StartsWith("PET_SKIN") ?? true);

        public static Func<DBItem, bool> IsPet => item => (item?.Tag?.StartsWith("PET_") ?? false)
        && !(item?.Tag?.StartsWith("PET_ITEM") ?? true)
        && !(item?.Tag?.StartsWith("PET_SKIN") ?? true);
    }
}
