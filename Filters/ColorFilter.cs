using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter
{
    [FilterDescription("Hex colors or absolute RGB distance: F2DF11-11 matches F0DE09 (sum of channel differences, not Delta E). Repeating patterns: pattern:ABCABC matches D07D07; pattern:AABBCC matches 1122FF; _E_E_E matches 1A2A3A. Repeated letters match equal digits; _ matches any digit. Bare AABBCC is an exact color.")]
    public class ColorFilter : NBTFilter
    {
        public override FilterType FilterType => FilterType.Equal;
        protected override string PropName => "color";

        public override Expression<Func<IDbItem, bool>> GetExpression(FilterArgs args)
        {
            var stringVal = args.Get(this);
            if (stringVal.StartsWith("pattern:", StringComparison.OrdinalIgnoreCase) || stringVal.Contains('_'))
                return PatternExpression(args, stringVal);
            if (stringVal.Contains('-'))
                return ToleranceExpression(args, stringVal);
            var val = new List<long>();
            if (stringVal.Contains(":"))
                val.Add(NBT.GetColor(stringVal));
            else // values are shifted a byte because the NBT.GetColor also mistakenly did that
                val.AddRange(stringVal.Split(',', ' ').Where(v => !string.IsNullOrWhiteSpace(v)).Select(v => FromHex(v)));
            //                val |=((long)0xFFFFFFFFF000000<<8);
            if (!args.TargetsDB)
                return a => (a as SaveAuction).FlatenedNBT.Where(n => n.Key == PropName).Select(n => NBT.GetColor(n.Value)).Intersect(val).Any();
            var key = args.NbtIntance.GetKeyId("color");
            return a => a.NBTLookup.Where(l => l.KeyId == key && val.Contains(l.Value)).Any();
        }

        private Expression<Func<IDbItem, bool>> ToleranceExpression(FilterArgs args, string input)
        {
            var match = Regex.Match(input.Trim(), @"^#?([0-9a-fA-F]{6})-([0-9]+)$");
            if (!match.Success || !int.TryParse(match.Groups[2].Value, out var distance) || distance > 765)
                throw new CoflnetException("invalid_color_tolerance", "Use hex-distance, e.g. AAAAAA-3, with an absolute RGB distance from 0 to 765");
            var center = Convert.ToInt32(match.Groups[1].Value, 16);
            var red = (long)(center >> 16);
            var green = (long)((center >> 8) & 255);
            var blue = (long)(center & 255);
            // Mask before dividing to handle the legacy signed, byte-shifted encoding.
            return ColorValueExpression(args, v => Math.Abs((v & 0xFF000000L) / 0x1000000 - red)
                + Math.Abs((v & 0xFF0000L) / 0x10000 - green)
                + Math.Abs((v & 0xFF00L) / 0x100 - blue) <= distance);
        }

        private Expression<Func<IDbItem, bool>> PatternExpression(FilterArgs args, string pattern)
        {
            pattern = pattern.Trim();
            if (pattern.StartsWith("pattern:", StringComparison.OrdinalIgnoreCase))
                pattern = pattern.Substring(8);
            pattern = pattern.ToUpperInvariant();
            if (pattern.Length != 6 || pattern.Any(c => c != '_' && (c < 'A' || c > 'Z')))
                throw new CoflnetException("invalid_color_pattern", "Color patterns need six letters or underscores, e.g. pattern:AABBCC or _E_E_E");

            var value = Expression.Parameter(typeof(long), "color");
            return ColorValueExpression(args, Expression.Lambda<Func<long, bool>>(PatternBody(value, pattern), value));
        }

        private Expression<Func<IDbItem, bool>> ColorValueExpression(FilterArgs args, Expression<Func<long, bool>> predicate)
        {
            if (!args.TargetsDB)
            {
                var matches = predicate.Compile();
                return a => MatchesColor(a as SaveAuction, matches);
            }

            var item = Expression.Parameter(typeof(IDbItem), "a");
            var lookup = Expression.Parameter(typeof(NBTLookup), "l");
            var keyMatches = Expression.Equal(Expression.Property(lookup, nameof(NBTLookup.KeyId)),
                Expression.Constant(args.NbtIntance.GetKeyId(PropName)));
            var visitor = new SubstExpressionVisitor();
            visitor.subst[predicate.Parameters[0]] = Expression.Property(lookup, nameof(NBTLookup.Value));
            var body = Expression.AndAlso(keyMatches, visitor.Visit(predicate.Body));
            return Expression.Lambda<Func<IDbItem, bool>>(Expression.Call(typeof(Enumerable), nameof(Enumerable.Any),
                new[] { typeof(NBTLookup) }, Expression.Property(item, nameof(IDbItem.NBTLookup)),
                Expression.Lambda<Func<NBTLookup, bool>>(body, lookup)), item);
        }

        private static Expression PatternBody(Expression value, string pattern)
        {
            Expression body = Expression.Constant(true);
            for (var i = 0; i < pattern.Length; i++)
            {
                var first = pattern.IndexOf(pattern[i]);
                if (pattern[i] == '_' || first == i)
                    continue;
                // Keep masks positive even for signed stored colors, and include the legacy byte offset.
                var left = Expression.And(value, Expression.Constant(15L << (28 - first * 4)));
                var right = Expression.Multiply(Expression.And(value, Expression.Constant(15L << (28 - i * 4))),
                    Expression.Constant(1L << ((i - first) * 4)));
                var equal = Expression.Equal(left, right);
                body = body is ConstantExpression ? equal : Expression.AndAlso(body, equal);
            }
            return body;
        }

        private bool MatchesColor(SaveAuction auction, Func<long, bool> matches)
        {
            return auction?.FlatenedNBT != null && auction.FlatenedNBT.TryGetValue(PropName, out var color)
                && matches(NBT.GetColor(color));
        }

        public long FromHex(string args)
        {
            return Convert.ToInt32(Regex.Replace(args, "[^0-9A-Fa-f]", ""), 16) << 8;
        }
        public string ToHex(string dec)
        {
            var color = NBT.GetColor(dec.TrimStart('#'));
            color = color >> 8 & 0xFFFFFF;
            return color.ToString("X6");
        }
    }
}
