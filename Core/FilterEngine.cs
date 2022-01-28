using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using hypixel;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Coflnet.Sky.Filter
{
    public class FilterEngine
    {
        private FilterDictonary Filters = new FilterDictonary();

        public IEnumerable<IFilter> AvailableFilters => Filters.Values;

        public HashSet<string> IgnoredKeys = new HashSet<string>() { "t" };

        public FilterEngine()
        {
            Filters.Add<ReforgeFilter>();
            Filters.Add<RarityFilter>();
            Filters.Add<PetLevelFilter>();
            Filters.Add<PetItemFilter>();
            Filters.Add<PetSkinFilter>();
            Filters.Add<CakeYearFilter>();
            Filters.Add<CandyFilter>();

            Filters.Add<BinFilter>();
            Filters.Add<StarsFilter>();
            Filters.Add<RecombobulatedFilter>();
            Filters.Add<HotPotatoCountFilter>();
            Filters.Add<EnchantmentFilter>();
            Filters.Add<EnchantLvlFilter>();
            Filters.Add<SecondEnchantmentFilter>();
            Filters.Add<SecondEnchantLvlFilter>();
            Filters.Add<ColorFilter>();
            Filters.Add<SellerFilter>();
            // skins
            Filters.Add<DragonArmorSkinFilter>();
            Filters.Add<ReaperMaskSkinFilter>();
            Filters.Add<SnowSuiteSkinFilter>();
            Filters.Add<TarantulaHelmetSkinFilter>();
            Filters.Add<FrozenBlazeSkinFilter>();
            Filters.Add<PerfectHelmetSkinFilter>();
            Filters.Add<DiversMaskSkinFilter>();
            Filters.Add<ShadowAssasinSkinFilter>();

            Filters.Add<JyrreMaxFilter>();
            Filters.Add<UIdFilter>();
            Filters.Add<CapturedPlayerFilter>();
            Filters.Add<ArtOfTheWarFilter>();
            Filters.Add<WoodSingularityFilter>();
            Filters.Add<EndBeforeFilter>();
            Filters.Add<EndAfterFilter>();
            Filters.Add<ItemIdFilter>();
            Filters.Add<EverythingFilter>();
        }

        /// <summary>
        /// Adds filters to a queryable
        /// </summary>
        /// <param name="query"></param>
        /// <param name="filters"></param>
        /// <param name="targetsDB">true when the query targets the database</param>
        /// <returns></returns>
        public IQueryable<SaveAuction> AddFilters(IQueryable<SaveAuction> query, Dictionary<string, string> filters, bool targetsDB)
        {
            var args = new FilterArgs(filters, targetsDB);
            foreach (var filter in filters)
            {
                if (IgnoredKeys.Contains(filter.Key))
                    continue;
                if (!Filters.TryGetValue(filter.Key, out IFilter filterObject))
                    throw new CoflnetException("filter_unknown", $"The filter {filter.Key} is not know, please remove it");
                query = filterObject.AddQuery(query, args);
            }

            return query;
        }


        public IQueryable<SaveAuction> AddFilters(IQueryable<SaveAuction> query, Dictionary<string, string> filters)
        {
            return AddFilters(query, filters, true);
        }

        public IEnumerable<SaveAuction> Filter(IEnumerable<SaveAuction> items, Dictionary<string, string> filters)
        {
            var args = new FilterArgs(filters, false);
            foreach (var filter in filters)
            {
                if (!Filters.TryGetValue(filter.Key, out IFilter filterObject))
                    throw new CoflnetException("filter_unknown", $"The filter {filter.Key} is not know, please remove it");
                items = filterObject.Filter(items, args);
            }

            return items;
        }

        public Func<SaveAuction, bool> GetMatcher(Dictionary<string, string> filters)
        {

            var args = new FilterArgs(filters, false);
            System.Linq.Expressions.Expression<Func<SaveAuction, bool>> expression = null;
            foreach (var filter in filters)
            {
                if (!Filters.TryGetValue(filter.Key, out IFilter filterObject))
                    throw new CoflnetException("filter_unknown", $"The filter {filter.Key} is not know, please remove it");
                var nextPart = (filterObject as GeneralFilter).GetExpression(args);
                if (nextPart == null)
                    continue;
                if (expression == null)
                    expression = nextPart;
                else
                    expression = expression.And(nextPart);
            }
            if (expression == null)
                return a => true;
            return expression.Compile();
        }

        public IEnumerable<IFilter> FiltersFor(DBItem item)
        {
            try
            {
                return Filters.Values.Where(f =>
                {
                    try
                    {
                        return f.IsApplicable(item);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Could not get filter {f.Name} for Item {item.Id} {item.Tag}. \n{e.StackTrace}");
                        return false;
                    }
                });
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not get filter for Item {item.Id} {item.Tag}. \n{e.StackTrace}");
                return new IFilter[0];
            }
        }

        public IFilter GetFilter(string name)
        {
            if (!Filters.TryGetValue(name, out IFilter value))
                throw new CoflnetException("unknown_filter", $"There is no filter with name {name}");
            return value;
        }
    }
    public static class PredicateBuilder
    {

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> a, Expression<Func<T, bool>> b)
        {

            ParameterExpression p = a.Parameters[0];

            SubstExpressionVisitor visitor = new SubstExpressionVisitor();
            visitor.subst[b.Parameters[0]] = p;

            Expression body = Expression.AndAlso(a.Body, visitor.Visit(b.Body));
            return Expression.Lambda<Func<T, bool>>(body, p);
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> a, Expression<Func<T, bool>> b)
        {

            ParameterExpression p = a.Parameters[0];

            SubstExpressionVisitor visitor = new SubstExpressionVisitor();
            visitor.subst[b.Parameters[0]] = p;

            Expression body = Expression.OrElse(a.Body, visitor.Visit(b.Body));
            return Expression.Lambda<Func<T, bool>>(body, p);
        }
    }

    internal class SubstExpressionVisitor : System.Linq.Expressions.ExpressionVisitor
    {
        public Dictionary<Expression, Expression> subst = new Dictionary<Expression, Expression>();

        protected override Expression VisitParameter(ParameterExpression node)
        {
            Expression newValue;
            if (subst.TryGetValue(node, out newValue))
            {
                return newValue;
            }
            return node;
        }
    }
}
