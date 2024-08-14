using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Coflnet.Sky.Core;
using Coflnet.Sky.Items.Client.Api;
using Coflnet.Sky.Items.Client.Model;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Coflnet.Sky.Filter
{
    public class FilterEngine
    {
        private FilterDictonary Filters = new FilterDictonary();

        public IEnumerable<IFilter> AvailableFilters => Filters.Values;

        public HashSet<string> IgnoredKeys = new HashSet<string>() { "t" };
        public static ImmutableHashSet<string> AttributeKeys = Constants.AttributeKeys;

        public FilterEngine()
        {
            Filters.Add<ReforgeFilter>();
            Filters.Add<RarityFilter>();
            Filters.Add<PetLevelFilter>();
            Filters.Add<PetItemFilter>();
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
            // skins
            Filters.Add<SkinFilter>();
            Filters.Add<PetSkinFilter>();
            Filters.Add<DragonArmorSkinFilter>();
            Filters.Add<ReaperMaskSkinFilter>();
            Filters.Add<SnowSuiteSkinFilter>();
            Filters.Add<TarantulaHelmetSkinFilter>();
            Filters.Add<FrozenBlazeSkinFilter>();
            Filters.Add<PerfectHelmetSkinFilter>();
            Filters.Add<DiversMaskSkinFilter>();
            Filters.Add<ShadowAssasinSkinFilter>();

            // runes
            Filters.Add<MusicRuneFilter>();
            Filters.Add<EnchantRuneFilter>();
            Filters.Add<TidalRuneFilter>();
            Filters.Add<EndRuneFilter>();
            //Filters.Add(new GeneralRuneFilter("GRAND_SEARING"));

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
            Filters.Add<BlazeConsumerFilter>();


            Filters.Add<StartingBidFilter>();
            Filters.Add<PricePerUnitFilter>();
            Filters.Add<HighestBidFilter>();
            Filters.Add<CountFilter>();
            Filters.Add<JyrreMaxFilter>();
            Filters.Add<UIdFilter>();
            Filters.Add<CapturedPlayerFilter>();
            Filters.Add<CakeOwnerFilter>();
            Filters.Add<ArtOfTheWarFilter>();
            Filters.Add<WoodSingularityFilter>();
            Filters.Add<EndBeforeFilter>();
            Filters.Add<EndAfterFilter>();
            Filters.Add<ItemCreatedBeforeFilter>();
            Filters.Add<ItemCreatedAfterFilter>();
            Filters.Add<ItemIdFilter>();
            Filters.Add<ItemTagFilter>();
            Filters.Add<UnlockedSlotsFilter>();
            Filters.Add<UnlockedSlotsMatchFilter>();
            Filters.Add<PerfectGemsCountFilter>();
            Filters.Add<FlawlessGemsCountFilter>();
            Filters.Add<EverythingFilter>();
            Filters.Add<DyeItemFilter>();

            Filters.Add<PartyHatYearFilter>();
            Filters.Add<PartyHatColorFilter>();
            Filters.Add<PartyHatEmojiFilter>();

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
            Filters.Add<BlocksBrokenFilter>();
            Filters.Add<ThunderCharge>();
            Filters.Add<IntelligenceBonus>();
            Filters.Add<SoldFilter>();
            Filters.Add<ItemNameContainsFilter>();
            Filters.Add<HasAttribute>();
            Filters.Add<PetExpFilter>();
            Filters.Add<CostPerExpPlusBaseFilter>();
            Filters.Add<TalismanEnrichmentFilter>();
            Filters.Add<HandlesFoundFilter>();
            Filters.Add<PowerAbilityScrollFilter>();
            Filters.Add<ItemTierFilter>();
            Filters.Add<NoOtherValuableEnchantsFilter>();
            Filters.Add<PowderCoatingFilter>();

            var gemGroups = new string[] { "COMBAT", "OFFENSIVE", "DEFENSIVE", "MINING_", "UNIVERSAL", "CHISEL" };
            foreach (var item in new string[] {
                "RUBY", "JASPER", "JADE", "TOPAZ", "AMETHYST", "AMBER", "SAPPHIRE", "OPAL", "ONYX", "PERIDOT", "AQUAMARINE", "CITRINE"
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

            Filters.TryAdd($"ultimate_duplex", new EnchantBaseFilter(Enchantment.EnchantmentType.ultimate_duplex, "ultimate_duplex"));
            Filters.TryAdd($"ultimate_reiterate", new EnchantBaseFilter(Enchantment.EnchantmentType.ultimate_reiterate, "ultimate_reiterate"));
            foreach (var item in Enum.GetValues<Enchantment.EnchantmentType>())
            {
                Filters.TryAdd($"{item}", new EnchantBaseFilter(item));
            }
        }

        /// <summary>
        /// Adds filters to a queryable
        /// </summary>
        /// <param name="query"></param>
        /// <param name="filters"></param>
        /// <param name="targetsDB">true when the query targets the database</param>
        /// <returns></returns>
        public IQueryable<IDbItem> AddFilters(IQueryable<IDbItem> query, Dictionary<string, string> filters, bool targetsDB)
        {
            var args = new FilterArgs(filters, targetsDB, this);
            foreach (var filter in filters)
            {
                if (IgnoredKeys.Contains(filter.Key))
                    continue;
                if (!Filters.TryGetValue(filter.Key, out IFilter filterObject))
                    throw new UnknownFilterException(filter.Key);
                query = filterObject.AddQuery(query, args);
            }

            return query;
        }

        public async Task Load(IServiceProvider provider)
        {
            Task<List<ItemPreview>> namesTask = LoadItemNames(provider);
            foreach (var filter in Filters)
            {
                await filter.Value.LoadData(provider);
            }
            var names = await namesTask;
            foreach (var item in names)
            {
                if (!item.Tag.StartsWith("RUNE_") && !item.Tag.StartsWith("UNIQUE_RUNE_"))
                    continue;
                var name = item.Name.Replace(" Rune I", "").Replace("◆", "").Replace(" ", "").TrimEnd('I') + "Rune";
                if (Filters.ContainsKey(name))
                    continue;
                var propName = item.Tag.Replace("UNIQUE_", "");
                Filters.Add(new GeneralRuneFilter(propName, name));
            }
        }

        private static async Task<List<ItemPreview>> LoadItemNames(IServiceProvider provider)
        {
            try
            {
                return await provider.GetService<IItemsApi>().ItemNamesGetAsync();
            }
            catch (Exception)
            {
                return new List<ItemPreview>();
            }
        }

        public IQueryable<SaveAuction> AddFilters(IQueryable<SaveAuction> query, Dictionary<string, string> filters)
        {
            return AddFilters(query, filters, true).Cast<SaveAuction>();
        }

        public IQueryable<IDbItem> AddInterfaceFilters(IQueryable<IDbItem> query, Dictionary<string, string> filters)
        {
            return AddFilters(query, filters, true);
        }

        public IEnumerable<SaveAuction> Filter(IEnumerable<SaveAuction> items, Dictionary<string, string> filters)
        {
            var args = new FilterArgs(filters, false);
            foreach (var filter in filters)
            {
                if (!Filters.TryGetValue(filter.Key, out IFilter filterObject))
                    throw new UnknownFilterException(filter.Key);
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
            Expression<Func<IDbItem, bool>> expression = null;
            foreach (var filter in filters)
            {
                Expression<Func<IDbItem, bool>> nextPart = GetExpression(args, filter);
                if (nextPart == null)
                    continue;
                if (expression == null)
                    expression = nextPart;
                else
                    expression = expression.And(nextPart);
            }
            if (expression == null)
                return a => true;
            // this may not work
            var converted = Expression.Convert(expression.Body, typeof(bool));
            return Expression.Lambda<Func<SaveAuction, bool>>(converted, expression.Parameters);
        }

        private Expression<Func<IDbItem, bool>> GetExpression(FilterArgs args, KeyValuePair<string, string> filter)
        {
            if (!Filters.TryGetValue(filter.Key, out IFilter filterObject))

                if (filter.Key.EndsWith("Rune"))
                {
                    var instance = new GeneralRuneFilter("RUNE_" + filter.Key.Replace("Rune", "").ToUpper(), filter.Key);
                    return instance.GetExpression(args);
                }
                else
                    throw new UnknownFilterException(filter.Key);
            var nextPart = (filterObject as GeneralFilter).GetExpression(args);
            return nextPart;
        }

        /// <summary>
        /// Debug helper to see which subexpression matched
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public IEnumerable<Expression<Func<IDbItem, bool>>> GetExpressions(Dictionary<string, string> filters)
        {
            if (filters == null)
                return new List<Expression<Func<IDbItem, bool>>>();
            var args = new FilterArgs(filters, false);
            var expressions = new List<Expression<Func<IDbItem, bool>>>();
            foreach (var filter in filters)
            {
                Expression<Func<IDbItem, bool>> nextPart = GetExpression(args, filter);
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
