using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter
{
    public class CandyFilter : PetFilter
    {
        public override FilterType FilterType => FilterType.Equal;

        public override IEnumerable<object> Options => new object[] { "none", "any", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" };

        public override Expression<System.Func<IDbItem, bool>> GetExpression(FilterArgs args)
        {
            var key = NBT.Instance.GetKeyId("candyUsed");
            var stringVal = args.Get(this);
            if (int.TryParse(stringVal, out int val))
                return a => a.NBTLookup.Where(l => l.KeyId == key && l.Value == val).Any();
            if (stringVal == "any")
                return a => a.NBTLookup.Where(l => l.KeyId == key && l.Value != 0).Any();
            return a => a.NBTLookup.Where(l => l.KeyId == key && l.Value == 0).Any();
        }
    }
}

