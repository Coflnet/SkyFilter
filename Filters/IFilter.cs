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
        Func<Items.Client.Model.Item, bool> IsApplicable { get; }
        FilterType FilterType { get; }

        IEnumerable<object> OptionsGet(OptionValues options);
        IQueryable<IDbItem> AddQuery(IQueryable<IDbItem> query, FilterArgs args);

        IEnumerable<SaveAuction> Filter(IEnumerable<SaveAuction> items, FilterArgs args);
        Task LoadData(IServiceProvider provider);
    }
}
