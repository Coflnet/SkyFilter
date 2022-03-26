using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Coflnet.Sky.Core;
using System;
using System.Linq.Expressions;

namespace Coflnet.Sky.Filter
{
    public class DragonArmorSkinFilter : SkinFilter
    {
        public override FilterType FilterType => FilterType.Equal;

        public override IEnumerable<object> Options => ItemDetails.Instance.TagLookup.Keys.Where(k => k.EndsWith("_SHIMMER") || k.EndsWith("_BABY"));

        public override Func<DBItem, bool> IsApplicable => item => item.Tag.EndsWith("_DRAGON_HELMET");
    }
}
