using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Coflnet.Sky.Core;
using System.Linq.Expressions;

namespace Coflnet.Sky.Filter
{
    public class PetItemFilter : PetFilter
    {
        public override FilterType FilterType => FilterType.Equal;
        public override IEnumerable<object> Options => ItemDetails.Instance.TagLookup.Keys.Where(k => k.StartsWith("PET_ITEM"))
            .Append("DWARF_TURTLE_SHELMET").Append("MINOS_RELIC").Append("CROCHET_TIGER_PLUSHIE");

        public override Expression<Func<SaveAuction, bool>> GetExpression(FilterArgs args)
        {
            var item = ItemDetails.Instance.GetItemIdForName(args.Get(this));
            var key = NBT.Instance.GetKeyId("heldItem");
            return a => a.NBTLookup.Where(l => l.KeyId == key && l.Value == item).Any();
        }
    }
}
