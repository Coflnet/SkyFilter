using System;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter
{
    public abstract class PetFilter : GeneralFilter
    {
        public override Func<Coflnet.Sky.Items.Client.Model.Item, bool> IsApplicable => IsPet;

        public static Func<Coflnet.Sky.Items.Client.Model.Item, bool> IsPet => item => (item?.Tag?.StartsWith("PET_") ?? false)
        && !(item?.Tag?.StartsWith("PET_ITEM") ?? true)
        && !(item?.Tag?.StartsWith("PET_SKIN") ?? true);
    }
}
