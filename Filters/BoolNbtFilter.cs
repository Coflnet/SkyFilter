using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter
{
    public abstract class BoolNbtFilter : GeneralFilter
    {
        public override FilterType FilterType => FilterType.Equal;
        public override IEnumerable<object> Options => new object[] { "yes", "no" };


        public override Func<Coflnet.Sky.Items.Client.Model.Item, bool> IsApplicable =>  a
            => a.Modifiers.Any(m=>m.Slug == Key);

        public abstract string Key {get; }

        public override Expression<Func<SaveAuction, bool>> GetExpression(FilterArgs args)
        {
            var key = NBT.Instance.GetKeyId(Key);
            var stringVal = args.Get(this);
            if (args.Get(this) == "true" || args.Get(this) == "yes" || args.Get(this) == "1")
                return a => a.NBTLookup.Where(l => l.KeyId == key).Any();
            return a => !a.NBTLookup.Where(l => l.KeyId == key).Any();
        }
    }
}
