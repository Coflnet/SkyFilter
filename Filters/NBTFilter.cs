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
            var key = args.NbtIntance.GetKeyId(PropName);
            if (stringValue.ToLower() == None.ToLower())
                return a => !a.NBTLookup.Where(l => l.KeyId == key).Any();
            if (stringValue.ToLower() == Any.ToLower())
                return a => a.NBTLookup.Where(l => l.KeyId == key).Any();
            long value = GetValueLong(stringValue, key, args);
            return a => a.NBTLookup.Where(l => l.KeyId == key && l.Value == value).Any();
        }

        protected virtual long GetValueLong(string stringValue, short key, FilterArgs args)
        {
            return args.NbtIntance.GetValueId(key, stringValue);
        }

        protected Expression<Func<IDbItem, bool>> NoDb(string stringValue)
        {
            if (stringValue.ToLower() == None.ToLower())
                return a => Select(a) == null && ItemCheck()(a as SaveAuction);
            if (stringValue.ToLower() == Any.ToLower())
                return a => Select(a) != null && ItemCheck()(a);

            return a => Select(a) == stringValue && ItemCheck()(a);
        }

        private string Select(IDbItem dbItem)
        {
            if (dbItem is not SaveAuction auction)
            {
                return null;
            }
            foreach (var nbt in auction.FlatenedNBT)
            {
                if (nbt.Key == PropName)
                    return nbt.Value;
            }
            return null;
        }

        protected virtual Func<IDbItem, bool> ItemCheck()
        {
            return a => true;
        }
    }
}

