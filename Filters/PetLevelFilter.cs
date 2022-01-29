using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using hypixel;
using System.Text.RegularExpressions;
using System.Linq.Expressions;

namespace Coflnet.Sky.Filter
{
    public class PetLevelOldFilter : PetFilter
    {
        public override FilterType FilterType => FilterType.Equal | FilterType.NUMERICAL | FilterType.RANGE;
        public override IEnumerable<object> Options => new object[] { 1, 200 };

        public override Expression<Func<SaveAuction, bool>> GetExpression(FilterArgs args)
        {
            var level = args.Get(this).Replace("X", "_").Replace("x", "_");
            if (!new Regex(@"^(1?[\dxX_]{1,2}|200)$").IsMatch(level))
                throw new CoflnetException("invalid_pet_level", "The pased pet level is invalid. Only numbers from 1-200");
            if (args.TargetsDB)
                return a => EF.Functions.Like(a.ItemName, $"[Lvl {level}]%");
            return a => a.ItemName.StartsWith($"[Lvl {level}]");
        }
    }
}
