using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Coflnet.Sky.Core;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Coflnet.Sky.Filter
{
    public class FilterEngine
    {
        private FilterDictonary Filters = new FilterDictonary();

        public IEnumerable<IFilter> AvailableFilters => Filters.Values;

        public HashSet<string> IgnoredKeys = new HashSet<string>() { "t" };
        public static string[] AttributeKeys = new string[]{
                "lifeline", "breeze", "speed", "experience", "mana_pool",
                "life_regeneration", "blazing_resistance", "arachno_resistance",
                "undead_resistance",
                "blazing_fortune", "fishing_experience", "double_hook", "infection",
                "trophy_hunter", "fisherman", "hunter", "fishing_speed",
                "life_recovery", "ignition", "combo", "attack_speed", "midas_touch",
                "mana_regeneration", "veteran", "mending", "ender_resistance", "dominance", "ender", "mana_steal", "blazing",
                "elite", "arachno", "undead",
                "warrior", "deadeye", "fortitude", "magic_find"
                };

        public FilterEngine()
        {
            Filters.Add<ReforgeFilter>();
            Filters.Add<RarityFilter>();
            Filters.Add<PetLevelFilter>();
            Filters.Add<PetItemFilter>();
            Filters.Add<PetSkinFilter>();
            Filters.Add<CakeYearFilter>();
            Filters.Add<CandyFilter>();
            Filters.Add<CleanFilter>();
            Filters.Add<PricePerLevelFilter>();

            Filters.Add<BinFilter>();
            Filters.Add<StarsFilter>();
            Filters.Add<IsShinyFilter>();
            Filters.Add<RecombobulatedFilter>();
            Filters.Add<HotPotatoCountFilter>();
            Filters.Add<EnchantmentFilter>();
            Filters.Add<EnchantLvlFilter>();
            Filters.Add<SecondEnchantmentFilter>();
            Filters.Add<SecondEnchantLvlFilter>();
            Filters.Add<ColorFilter>();
            Filters.Add<ExoticColorFilter>();
            Filters.Add<FairyColorFilter>();
            Filters.Add<CrystalColorFilter>();
            Filters.Add<HexColorListFilter>();
            Filters.Add<CrabHatColorFilter>();
            Filters.Add<SellerFilter>();
            Filters.Add<WinningBidFilter>();
            Filters.Add<EthermergeFilter>();
            Filters.Add<FraggedFilter>();
            // skins
            Filters.Add<DragonArmorSkinFilter>();
            Filters.Add<ReaperMaskSkinFilter>();
            Filters.Add<SnowSuiteSkinFilter>();
            Filters.Add<TarantulaHelmetSkinFilter>();
            Filters.Add<FrozenBlazeSkinFilter>();
            Filters.Add<PerfectHelmetSkinFilter>();
            Filters.Add<DiversMaskSkinFilter>();
            Filters.Add<ShadowAssasinSkinFilter>();
            Filters.Add<SkinFilter>();

            // runes
            Filters.Add<MusicRuneFilter>();
            Filters.Add<EnchantRuneFilter>();
            Filters.Add<TidalRuneFilter>();
            Filters.Add<EndRuneFilter>();
            Filters.Add(new GeneralRuneFilter("GRAND_SEARING"));

            // kills
            Filters.Add<ZombieKillsFilter>();
            Filters.Add<SpiderKillsFilter>();
            Filters.Add<EmanKillsFilter>();
            Filters.Add<ExpertiseKillsFilter>();
            Filters.Add<RaiderKillsFilter>();
            Filters.Add<SwordKillsFilter>();
            Filters.Add<BloodGodKillsFilter>();
            Filters.Add<BlazeKillsFilter>();
            Filters.Add<YogsKilledFilter>();


            Filters.Add<StartingBidFilter>();
            Filters.Add<PricePerUnitFilter>();
            Filters.Add<HighestBidFilter>();
            Filters.Add<CountFilter>();
            Filters.Add<JyrreMaxFilter>();
            Filters.Add<UIdFilter>();
            Filters.Add<CapturedPlayerFilter>();
            Filters.Add<ArtOfTheWarFilter>();
            Filters.Add<WoodSingularityFilter>();
            Filters.Add<EndBeforeFilter>();
            Filters.Add<EndAfterFilter>();
            Filters.Add<ItemIdFilter>();
            Filters.Add<UnlockedSlotsFilter>();
            Filters.Add<UnlockedSlotsMatchFilter>();
            Filters.Add<PerfectGemsCountFilter>();
            Filters.Add<FlawlessGemsCountFilter>();
            Filters.Add<EverythingFilter>();
            Filters.Add<DyeItemFilter>();

            Filters.Add<PartyHatYearFilter>();
            Filters.Add<PartyHatColorFilter>();

            Filters.Add<EditionFilter>();
            Filters.Add<BaseStatBoostFilter>();
            Filters.Add<ManaDisintegrator>();
            Filters.Add<DrillPartEngineFilter>();
            Filters.Add<DrillPartFuelTankFilter>();
            Filters.Add<DrillPartUpgradeModuleFilter>();
            Filters.Add<AbilityScrollFilter>();
            Filters.Add<ArtOfPeaceFilter>();
            Filters.Add<ModelFilter>();
            Filters.Add<FarmedCultivatingFilter>();
            Filters.Add<FarmingForDummies>();
            Filters.Add<MinedCropsFilter>();
            Filters.Add<ThunderCharge>();
            Filters.Add<SoldFilter>();
            Filters.Add<ItemNameContainsFilter>();
            Filters.Add<HasAttribute>();

            var gemGroups = new string[] { "COMBAT", "OFFENSIVE", "DEFENSIVE", "MINING_", "UNIVERSAL" };
            foreach (var item in new string[] {
                "RUBY", "JASPER", "JADE", "TOPAZ", "AMETHYST", "AMBER", "SAPPHIRE", "OPAL",
                 }.Concat(gemGroups))
            {
                for (int i = 0; i < 2; i++)
                {
                    Filters.Add(new GemFilter($"{item}_{i}"));
                }
            }
            foreach (var item in gemGroups)
            {
                for (int i = 0; i < 2; i++)
                {
                    Filters.Add(new GemTypeFilter($"{item}_{i}"));
                }
            }
            Filters.Add(new GemFilter("AMETHYST_2"));

            foreach (var item in AttributeKeys)
            {
                var instance = new AttributeFilter(item, 0, 10);
                Filters.Add(item, instance);
            }
            // mending is called vitality in game
            Filters.Add("vitality", new AttributeFilter("mending", 0, 10, "vitality"));

            foreach (var item in Enum.GetValues<Enchantment.EnchantmentType>())
            {
                Filters.TryAdd($"{item}", new EnchantBaseFilter(item));
            }
            Filters.TryAdd($"ultimate_duplex", new EnchantBaseFilter(Enchantment.EnchantmentType.ultimate_duplex));
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
            var args = new FilterArgs(filters, targetsDB, this);
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

        public async Task Load(ServiceProvider provider)
        {
            foreach (var filter in Filters)
            {
                await filter.Value.LoadData(provider);
            }
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

            return GetMatchExpression(filters).Compile();
        }

        public Expression<Func<SaveAuction, bool>> GetMatchExpression(Dictionary<string, string> filters, bool targetsDb = false)
        {
            if (filters == null)
                return a => true;
            var args = new FilterArgs(filters, targetsDb, this);
            System.Linq.Expressions.Expression<Func<SaveAuction, bool>> expression = null;
            foreach (var filter in filters)
            {
                Expression<Func<SaveAuction, bool>> nextPart = GetExpression(args, filter);
                if (nextPart == null)
                    continue;
                if (expression == null)
                    expression = nextPart;
                else
                    expression = expression.And(nextPart);
            }
            if (expression == null)
                return a => true;
            return expression;
        }

        private Expression<Func<SaveAuction, bool>> GetExpression(FilterArgs args, KeyValuePair<string, string> filter)
        {
            if (!Filters.TryGetValue(filter.Key, out IFilter filterObject))
                throw new CoflnetException("filter_unknown", $"The filter {filter.Key} is not know, please remove it");
            var nextPart = (filterObject as GeneralFilter).GetExpression(args);
            return nextPart;
        }

        /// <summary>
        /// Debug helper to see which subexpression matched
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public IEnumerable<Expression<Func<SaveAuction, bool>>> GetExpressions(Dictionary<string, string> filters)
        {
            if (filters == null)
                return new List<Expression<Func<SaveAuction, bool>>>();
            var args = new FilterArgs(filters, false);
            var expressions = new List<Expression<Func<SaveAuction, bool>>>();
            foreach (var filter in filters)
            {
                Expression<Func<SaveAuction, bool>> nextPart = GetExpression(args, filter);
                if (nextPart == null)
                    continue;
                expressions.Add(nextPart);
            }
            return expressions;
        }

        public IEnumerable<IFilter> FiltersFor(Coflnet.Sky.Items.Client.Model.Item item)
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

        public Dictionary<string, HashSet<string>> GetFilterOptions(Dictionary<string, List<string>> fromItemSerive)
        {
            try
            {
                return Filters.Values.Select(f =>
                {
                    try
                    {
                        return (f, f.OptionsGet(new OptionValues(fromItemSerive)));
                    }
                    catch (Exception e)
                    {
                        return (null, null);
                    }
                }).ToDictionary(f => f.f.Name, f => f.Item2.Select(o => o.ToString()).ToHashSet());
            }
            catch (Exception e)
            {
                return new Dictionary<string, HashSet<string>>();
            }
        }

        public IFilter GetFilter(string name)
        {
            if (!Filters.TryGetValue(name, out IFilter value))
                throw new CoflnetException("unknown_filter", $"There is no filter with name {name}");
            return value;
        }
    }

    public class OptionValues
    {
        public Dictionary<string, List<string>> Options;

        public OptionValues(Dictionary<string, List<string>> options)
        {
            Options = options;
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
