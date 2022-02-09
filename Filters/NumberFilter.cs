using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using hypixel;

namespace Coflnet.Sky.Filter
{
    public abstract class NumberFilter : GeneralFilter
    {
        public override FilterType FilterType => FilterType.NUMERICAL | FilterType.LOWER | FilterType.RANGE;
        public override IEnumerable<object> Options => new object[] { "0", Int32.MaxValue };

        public override Func<DBItem, bool> IsApplicable => a => true;


        public override Expression<Func<SaveAuction, bool>> GetExpression(FilterArgs args)
        {
            var content = args.Get(this);
            Expression<Func<SaveAuction, long>> selector = GetSelector(args);
            if(content.EndsWith("-"))
                content = ">" + content.TrimEnd('-');
            if (content.Contains("-"))
            {
                var parts = content.Split("-").Select(a => long.Parse(a)).ToArray();
                var min = GetLowerBound(args, parts[0]);
                var max = GetUpperBound(args, parts[1]);
                return ExpressionMinMax(selector, min, max);
            }
            if(content == "any")
            {
                content = ">0";
            }
            var value = long.Parse(content.Replace("<", "").Replace(">", ""));
            if (content.StartsWith("<"))
                return ExpressionMinMax(selector, 1, value -1);
            if (content.StartsWith(">"))
            {
                return Expression.Lambda<Func<SaveAuction, bool>>(
                    Expression.GreaterThan(
                        selector.Body,
                        Expression.Constant(GetUpperBound(args, value), typeof(long))
                    ), selector.Parameters);
            }

            return ExpressionMinMax(selector, GetLowerBound(args, value), GetUpperBound(args, value));
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

        public abstract Expression<Func<SaveAuction, long>> GetSelector(FilterArgs args);

        /// <summary>
        /// Remap the input in some way
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual long GetLowerBound(FilterArgs args, long input)
        {
            return input;
        }
        /// <summary>
        /// Remap the input in some way
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual long GetUpperBound(FilterArgs args, long input)
        {
            return input;
        }
    }
}
