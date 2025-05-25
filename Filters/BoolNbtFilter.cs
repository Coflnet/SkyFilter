using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter
{
    [FilterDescription("Is X applied onto the item")]
    public abstract class BoolNbtFilter : GeneralFilter
    {
        public override FilterType FilterType => FilterType.Equal;
        public override IEnumerable<object> Options => new object[] { "yes", "no" };


        public override Func<Coflnet.Sky.Items.Client.Model.Item, bool> IsApplicable =>  a
            => a.Modifiers.Any(m=>m.Slug == Key);

        public abstract string Key {get; }

        public override Expression<Func<IDbItem, bool>> GetExpression(FilterArgs args)
        {
            var shouldBePresent = args.Get(this) == "true" || args.Get(this) == "yes" || args.Get(this) == "1"|| args.Get(this).ToLower() == "any";
            if(!args.TargetsDB)
                return a => (a as SaveAuction).FlatenedNBT.Where(n => n.Key == Key).Any() == shouldBePresent;
            var key = args.NbtIntance.GetKeyId(Key);
            var stringVal = args.Get(this);
            if (shouldBePresent)
                return a => a.NBTLookup.Where(l => l.KeyId == key).Any();
            return a => !a.NBTLookup.Where(l => l.KeyId == key).Any();
        }
    }
}
