using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using hypixel;
using System.Text.RegularExpressions;

namespace Coflnet.Sky.Filter
{
    public class PetLevelFilter : PetFilter
    {
        public override FilterType FilterType => FilterType.Equal | FilterType.NUMERICAL | FilterType.RANGE;
        public override IEnumerable<object> Options => new object[] { 1, 100 };

        public override IQueryable<SaveAuction> AddQuery(IQueryable<SaveAuction> query, FilterArgs args)
        {
            var level = args.Get(this).Replace("X","_").Replace("x","_");
            if(!new Regex(@"^(1?[\dxX_]{1,2}|200)$").IsMatch(level))
                throw new CoflnetException("invalid_pet_level","The pased pet level is invalid. Only numbers from 1-200");
            return query.Where(a => EF.Functions.Like(a.ItemName, $"[Lvl {level}]%"));
        }
    }
}
