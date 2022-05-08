using System;
using System.Collections.Generic;
using System.Linq;
using Coflnet.Sky.Core;
using System.Threading.Tasks;
using Coflnet.Sky.Items.Client.Model;

namespace Coflnet.Sky.Filter
{
    public interface IFilter
    {
        string Name { get; }
        /// <summary>
        /// Is this filter available for a given item?
        /// </summary>
        Func<Item, bool> IsApplicable { get; }
        FilterType FilterType { get; }
        IEnumerable<object> Options {get; }

        IQueryable<SaveAuction> AddQuery(IQueryable<SaveAuction> query, FilterArgs args);

        IEnumerable<SaveAuction> Filter(IEnumerable<SaveAuction> items, FilterArgs args);
        Task LoadData(IServiceProvider provider);
    }
}
