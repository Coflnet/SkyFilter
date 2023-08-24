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
        public override Func<Coflnet.Sky.Items.Client.Model.Item, bool> IsApplicable => item => item.Tag.EndsWith("_DRAGON_HELMET");

        protected override Func<IDbItem, bool> ItemCheck()
        {
            return a => a.Tag.EndsWith("_DRAGON_HELMET");
        }
    }
}
