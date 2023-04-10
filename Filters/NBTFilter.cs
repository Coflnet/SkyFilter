using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Coflnet.Sky.Core;
using Coflnet.Sky.Items.Client.Model;

namespace Coflnet.Sky.Filter
{
    public abstract class NBTFilter : GeneralFilter
    {
        protected abstract string PropName { get; }

        public static readonly string None = "None";
        public static readonly string Any = "Any";

        public override Func<Item, bool> IsApplicable => a
            => a.Modifiers.Any(m => m.Slug == PropName);

        public override FilterType FilterType => FilterType.Equal;

        public override IEnumerable<object> OptionsGet(OptionValues options)
        {
            return options.Options.GetValueOrDefault(PropName, new List<string>()).Append(None).Prepend(Any);
        }

        public override Expression<Func<SaveAuction, bool>> GetExpression(FilterArgs args)
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

        protected Expression<Func<SaveAuction, bool>> NoDb(string stringValue)
        {
            if (stringValue == None)
                return a => !a.FlatenedNBT.Where(v => v.Key == PropName).Any();
            if (stringValue == Any)
                return a => a.FlatenedNBT.Where(v => v.Key == PropName).Any();

            return a => a.FlatenedNBT.Where(v => v.Key == PropName && v.Value == stringValue).Any();
        }
    }
}

