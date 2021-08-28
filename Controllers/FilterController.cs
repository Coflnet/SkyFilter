using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using hypixel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Coflnet.Sky.Filter.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FilterController : ControllerBase
    {
        private readonly ILogger<FilterController> _logger;
        private static FilterEngine filter = new FilterEngine();

        public FilterController(ILogger<FilterController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Tests if the given auction matches a filter
        /// </summary>
        /// <param name="query"></param>
        /// <returns>true if the auction matches the filter</returns>
        [HttpPost]
        public bool MatchesFilter([FromBody] FilterQuery query)
        {
            var auctionList = new SaveAuction[1]{query.Auction}.AsQueryable<SaveAuction>();
            return filter.AddFilters(auctionList,query.Filters).Any();
        }

        public class FilterQuery
        {
            public Dictionary<string,string> Filters {get;set;}
            public SaveAuction Auction {get;set;}

        }
    }
}
