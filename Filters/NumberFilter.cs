using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter
{
    public abstract class NumberFilter : GeneralFilter
    {
        public override FilterType FilterType => FilterType.NUMERICAL | FilterType.RANGE;
        public override IEnumerable<object> Options => new object[] { "0", 10_000_000_000 };

        public override Func<Coflnet.Sky.Items.Client.Model.Item, bool> IsApplicable => a => true;


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
            if (content.StartsWith("<"))
                return ExpressionMinMaxInstance(selector, 1, GetUpperBound(args, value - 1));
            if (content.StartsWith(">"))
            {
                return ExpressionMinMaxInstance(selector, GetLowerBound(args, value + 1), long.MaxValue);
            }

            return ExpressionMinMaxInstance(selector, GetLowerBound(args, value), GetUpperBound(args, value));
        }

        protected virtual string GetValue(FilterArgs args)
        {
            return args.Get(this);
        }

        public virtual Expression<Func<T, bool>> ExpressionMinMaxInstance<T>(Expression<Func<T, long>> selector, long min, long max)
        {
            return ExpressionMinMax(selector, min, max);
        }

        public static Expression<Func<T, bool>> ExpressionMinMax<T>(Expression<Func<T, long>> selector, long min, long max)
        {
            return Expression.Lambda<Func<T, bool>>(
                Expression.And(
                    Expression.GreaterThanOrEqual(
                    Expression.Constant(max, typeof(long)),
                    selector.Body),
                    Expression.GreaterThanOrEqual(
                    selector.Body,
                    Expression.Constant(min, typeof(long))
                )), selector.Parameters
            );
        }

        public abstract Expression<Func<IDbItem, long>> GetSelector(FilterArgs args);

        /// <summary>
        /// Remap the input in some way
        /// </summary>
        /// <param name="args"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual long GetLowerBound(FilterArgs args, long input)
        {
            return input;
        }
        /// <summary>
        /// Remap the input in some way
        /// </summary>
        /// <param name="args"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual long GetUpperBound(FilterArgs args, long input)
        {
            return input;
        }

        protected virtual bool ContainsRangeRequest(string filterValue)
        {
            return filterValue.StartsWith("<") || filterValue.StartsWith(">") || filterValue.Contains('-');
        }
    }
}
