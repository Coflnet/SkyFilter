using System;
using System.Collections.Generic;
using System.Linq;
using hypixel;

namespace Coflnet.Sky.Filter
{
    public class FilterEngine
    {
        private FilterDictonary Filters = new FilterDictonary();

        public IEnumerable<IFilter> AvailableFilters => Filters.Values;

        public HashSet<string> IgnoredKeys = new HashSet<string>(){"t"};

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
                if(IgnoredKeys.Contains(filter.Key))
                    continue;
                if (!Filters.TryGetValue(filter.Key, out IFilter filterObject))
                    throw new CoflnetException("filter_unknown", $"The filter {filter.Key} is not know, please remove it");
                query = filterObject.AddQuery(query, args);
            }

            return query;
        }

        public IQueryable<SaveAuction> AddFilters(IQueryable<SaveAuction> query, Dictionary<string, string> filters)
        {
            return AddFilters(query,filters,true);
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

        public IEnumerable<IFilter> FiltersFor(DBItem item)
        {
            try 
            {
                return Filters.Values.Where(f=>{
                    try 
                    {
                        return f.IsApplicable(item);
                    } catch(Exception e)
                    {
                        Console.WriteLine($"Could not get filter {f.Name} for Item {item.Id} {item.Tag}. \n{e.StackTrace}");
                        return false;
                    }});
            } catch(Exception e)
            {
                Console.WriteLine($"Could not get filter for Item {item.Id} {item.Tag}. \n{e.StackTrace}");
                return new IFilter[0];
            }
        }

        public IFilter GetFilter(string name)
        {
            if(!Filters.TryGetValue(name, out IFilter value))
                throw new CoflnetException("unknown_filter",$"There is no filter with name {name}");
            return value;
        }
    }
}
