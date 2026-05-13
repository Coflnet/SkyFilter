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
        protected virtual IEnumerable<string> AlternatePropNames => Array.Empty<string>();

        public static readonly string None = "None";
        public static readonly string Any = "Any";

        public override Func<Items.Client.Model.Item, bool> IsApplicable => a
            => a.Modifiers.Any(m => GetSearchPropNames().Contains(m.Slug, StringComparer.OrdinalIgnoreCase));

        public override FilterType FilterType => FilterType.Equal;

        public override IEnumerable<object> OptionsGet(OptionValues options)
        {
            var values = GetSearchPropNames()
                .SelectMany(name => options.Options.GetValueOrDefault(name, new List<string>()))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            return values.Prepend(Any).Append(None).Cast<object>();
        }

        public override Expression<Func<IDbItem, bool>> GetExpression(FilterArgs args)
        {
            var stringValue = args.Get(this);
            var propNames = GetSearchPropNames();
            if (!args.TargetsDB)
                return NoDb(stringValue, propNames);

            var keys = propNames
                .Select(args.NbtIntance.GetKeyId)
                .Where(key => key > 0)
                .Distinct()
                .ToArray();

            if (stringValue.ToLower() == None.ToLower())
                return BuildAnyLookupExpression(keys, shouldExist: false);
            if (stringValue.ToLower() == Any.ToLower())
                return BuildAnyLookupExpression(keys, shouldExist: true);

            var keyValues = keys.Select(key =>
            {
                var value = GetValueLong(stringValue, key, args);
                if (value <= 0)
                    value = GetValueLong('"' + stringValue + '"', key, args);
                return (Key: key, Value: value);
            }).ToArray();

            return BuildValueLookupExpression(keyValues);
        }

        protected virtual long GetValueLong(string stringValue, short key, FilterArgs args)
        {
            return args.NbtIntance.GetValueId(key, stringValue);
        }

        protected virtual bool MatchesNoDbValue(string actualValue, string filterValue)
        {
            return actualValue == filterValue;
        }

        protected Expression<Func<IDbItem, bool>> NoDb(string stringValue, string[] propNames)
        {
            if (stringValue.ToLower() == None.ToLower())
                return a => Select(a, propNames) == null && ItemCheck()(a as SaveAuction);
            if (stringValue.ToLower() == Any.ToLower())
                return a => Select(a, propNames) != null && ItemCheck()(a);

            return a => MatchesNoDbValue(Select(a, propNames), stringValue) && ItemCheck()(a);
        }

        private string Select(IDbItem dbItem, IEnumerable<string> propNames)
        {
            if (dbItem is not SaveAuction auction)
            {
                return null;
            }

            if (auction.FlatenedNBT == null)
                return null;

            foreach (var propName in propNames)
            {
                if (auction.FlatenedNBT.TryGetValue(propName, out var value))
                    return value;
            }

            return null;
        }

        private string[] GetSearchPropNames()
        {
            return new[] { PropName }
                .Concat(AlternatePropNames ?? Array.Empty<string>())
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        private static Expression<Func<IDbItem, bool>> BuildAnyLookupExpression(short[] keys, bool shouldExist)
        {
            if (keys.Length == 0)
                return shouldExist ? a => false : a => true;

            var itemParameter = Expression.Parameter(typeof(IDbItem), "a");
            var lookupParameter = Expression.Parameter(typeof(NBTLookup), "l");
            var keyProperty = Expression.Property(lookupParameter, nameof(NBTLookup.KeyId));
            Expression predicate = null;

            foreach (var key in keys)
            {
                var condition = Expression.Equal(keyProperty, Expression.Constant(key));
                predicate = predicate == null ? condition : Expression.OrElse(predicate, condition);
            }

            return BuildLookupExpression(itemParameter, lookupParameter, predicate, shouldExist);
        }

        private static Expression<Func<IDbItem, bool>> BuildValueLookupExpression((short Key, long Value)[] keyValues)
        {
            if (keyValues.Length == 0)
                return a => false;

            var itemParameter = Expression.Parameter(typeof(IDbItem), "a");
            var lookupParameter = Expression.Parameter(typeof(NBTLookup), "l");
            var keyProperty = Expression.Property(lookupParameter, nameof(NBTLookup.KeyId));
            var valueProperty = Expression.Property(lookupParameter, nameof(NBTLookup.Value));
            Expression predicate = null;

            foreach (var keyValue in keyValues)
            {
                var keyMatches = Expression.Equal(keyProperty, Expression.Constant(keyValue.Key));
                var valueMatches = Expression.Equal(valueProperty, Expression.Constant(keyValue.Value));
                var condition = Expression.AndAlso(keyMatches, valueMatches);
                predicate = predicate == null ? condition : Expression.OrElse(predicate, condition);
            }

            return BuildLookupExpression(itemParameter, lookupParameter, predicate, shouldExist: true);
        }

        private static Expression<Func<IDbItem, bool>> BuildLookupExpression(ParameterExpression itemParameter, ParameterExpression lookupParameter, Expression predicate, bool shouldExist)
        {
            var lookupExpression = Expression.Property(itemParameter, nameof(IDbItem.NBTLookup));
            var anyExpression = Expression.Call(
                typeof(Enumerable),
                nameof(Enumerable.Any),
                new[] { typeof(NBTLookup) },
                lookupExpression,
                Expression.Lambda<Func<NBTLookup, bool>>(predicate, lookupParameter));

            Expression body = anyExpression;
            if (!shouldExist)
                body = Expression.Not(anyExpression);

            return Expression.Lambda<Func<IDbItem, bool>>(body, itemParameter);
        }

        protected virtual Func<IDbItem, bool> ItemCheck()
        {
            return a => true;
        }
    }
}

