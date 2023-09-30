using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter
{
    public abstract class NumberFilter : NumberFilter<long>
    {
        public override Expression<Func<IDbItem, bool>> GetExpression(FilterArgs args)
        {
            string content = GetValue(args);
            if (string.IsNullOrEmpty(content))
                content = "0";
            Expression<Func<IDbItem, long>> selector = GetSelector(args);
            if (content.EndsWith("-"))
                content = ">" + content.TrimEnd('-');
            if (content.Contains("-"))
            {
                var parts = content.Split("-").Select(a => NumberParser.Long(a)).ToArray();
                var min = GetLowerBound(args, parts[0]);
                var max = GetUpperBound(args, parts[1]);
                return ExpressionMinMaxInstance(selector, min, max);
            }
            if (content == "any")
            {
                content = ">0";
            }
            if (!NumberParser.TryLong(content.Replace("<", "").Replace(">", ""), out long value) && content.Length == 1)
                value = 1;
            var toMove = content.Contains("=") ? 0 : 1;
            if (content.StartsWith("<"))
                return ExpressionMinMaxInstance(selector, 1, GetUpperBound(args, value - toMove));
            if (content.StartsWith(">"))
            {
                return ExpressionMinMaxInstance(selector, GetLowerBound(args, value + toMove), long.MaxValue);
            }

            return ExpressionMinMaxInstance(selector, GetLowerBound(args, value), GetUpperBound(args, value));
        }
    }
    public abstract class DoubleNumberFilter : NumberFilter<double>
    {
        public override Expression<Func<IDbItem, bool>> GetExpression(FilterArgs args)
        {
            string content = GetValue(args);
            if (string.IsNullOrEmpty(content))
                content = "0";
            Expression<Func<IDbItem, double>> selector = GetSelector(args);
            if (content.EndsWith("-"))
                content = ">" + content.TrimEnd('-');
            if (content.Contains("-"))
            {
                var parts = content.Split("-").Select(a => NumberParser.Double(a)).ToArray();
                var min = GetLowerBound(args, parts[0]);
                var max = GetUpperBound(args, parts[1]);
                return ExpressionMinMaxInstance(selector, min, max);
            }
            if (content == "any")
            {
                content = ">0";
            }
            if (!NumberParser.TryDouble(content.Replace("<", "").Replace(">", ""), out double value) && content.Length == 1)
                value = 1;
            var toMove = content.Contains("=") ? 0 : 0.01;
            if (content.StartsWith("<"))
                return ExpressionMinMaxInstance(selector, 1, GetUpperBound(args, value - toMove));
            if (content.StartsWith(">"))
            {
                return ExpressionMinMaxInstance(selector, GetLowerBound(args, value + toMove), long.MaxValue);
            }

            return ExpressionMinMaxInstance(selector, GetLowerBound(args, value), GetUpperBound(args, value));
        }
    }

    public abstract class NumberFilter<B> : GeneralFilter where B : INumber<B>
    {
        public override FilterType FilterType => FilterType.NUMERICAL | FilterType.RANGE;
        public override IEnumerable<object> Options => new object[] { "0", 10_000_000_000 };

        public override Func<Coflnet.Sky.Items.Client.Model.Item, bool> IsApplicable => a => true;




        protected virtual string GetValue(FilterArgs args)
        {
            return args.Get(this);
        }

        public virtual Expression<Func<T, bool>> ExpressionMinMaxInstance<T>(Expression<Func<T, B>> selector, B min, B max)
        {
            return ExpressionMinMax(selector, min, max);
        }

        public static Expression<Func<T, bool>> ExpressionMinMax<T>(Expression<Func<T, B>> selector, B min, B max)
        {
            return Expression.Lambda<Func<T, bool>>(
                Expression.And(
                    Expression.GreaterThanOrEqual(
                    Expression.Constant(max, typeof(B)),
                    selector.Body),
                    Expression.GreaterThanOrEqual(
                    selector.Body,
                    Expression.Constant(min, typeof(B))
                )), selector.Parameters
            );
        }

        public abstract Expression<Func<IDbItem, B>> GetSelector(FilterArgs args);

        /// <summary>
        /// Remap the input in some way
        /// </summary>
        /// <param name="args"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual B GetLowerBound(FilterArgs args, B input)
        {
            return input;
        }
        /// <summary>
        /// Remap the input in some way
        /// </summary>
        /// <param name="args"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual B GetUpperBound(FilterArgs args, B input)
        {
            return input;
        }

        protected virtual bool ContainsRangeRequest(string filterValue)
        {
            return filterValue.StartsWith("<") || filterValue.StartsWith(">") || filterValue.Contains('-');
        }
    }
}
