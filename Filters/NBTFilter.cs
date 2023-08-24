using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter
{
    public abstract class NBTFilter : GeneralFilter
    {
        protected abstract string PropName { get; }

        public static readonly string None = "None";
        public static readonly string Any = "Any";

        public override Func<Items.Client.Model.Item, bool> IsApplicable => a
            => a.Modifiers.Any(m => m.Slug == PropName);

        public override FilterType FilterType => FilterType.Equal;

        public override IEnumerable<object> OptionsGet(OptionValues options)
        {
            return options.Options.GetValueOrDefault(PropName, new List<string>()).Append(None).Prepend(Any);
        }

        public override Expression<Func<IDbItem, bool>> GetExpression(FilterArgs args)
        {
            var stringValue = args.Get(this);
            if (!args.TargetsDB)
                return NoDb(stringValue);
            var key = NBT.Instance.GetKeyId(PropName);
            if (stringValue == None)
                return a => !a.NBTLookup.Where(l => l.KeyId == key).Any();
            if (stringValue == Any)
                return a => a.NBTLookup.Where(l => l.KeyId == key).Any();
            long value = GetValueLong(stringValue, key);
            return a => a.NBTLookup.Where(l => l.KeyId == key && l.Value == value).Any();
        }

        protected virtual long GetValueLong(string stringValue, short key)
        {
            return NBT.Instance.GetValueId(key, stringValue);
        }

        protected Expression<Func<IDbItem, bool>> NoDb(string stringValue)
        {
            if (stringValue.ToLower() == None.ToLower())
                return a => !(a as SaveAuction).FlatenedNBT.Where(v => v.Key == PropName).Any() && ItemCheck()(a as SaveAuction);
            if (stringValue.ToLower() == Any.ToLower())
                return a => (a as SaveAuction).FlatenedNBT.Where(v => v.Key == PropName).Any()  && ItemCheck()(a);

            return a => (a as SaveAuction).FlatenedNBT.Where(v => v.Key == PropName && v.Value == stringValue).Any();
        }

        protected virtual Func<IDbItem, bool> ItemCheck()
        {
            return a => true;
        }
    }
}

