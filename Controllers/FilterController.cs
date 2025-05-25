using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Coflnet.Sky.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Coflnet.Sky.Filter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FilterController : ControllerBase
    {
        private readonly ILogger<FilterController> _logger;
        private  FilterEngine filter;

        public FilterController(ILogger<FilterController> logger, FilterEngine filter)
        {
            _logger = logger;
            this.filter = filter;
        }

        /// <summary>
        /// Tests if the given auction matches a filter
        /// </summary>
        /// <param name="query"></param>
        /// <returns>true if the auction matches the filter</returns>
        [HttpPost]
        public bool MatchesFilter([FromBody] FilterQuery query)
        {
            return filter.GetMatchExpression(query.Filters).Compile()(query.Auction);
        }

        public class FilterQuery
        {
            public Dictionary<string, string> Filters { get; set; }
            public ApiSaveAuction Auction { get; set; }

        }

        public class ApiSaveAuction : SaveAuction
        {
            /// <summary>
            /// 
            /// </summary>
            [DataMember(Name = "flatNbt", EmitDefaultValue = true)]
            public override Dictionary<string, string> FlatenedNBT { get; set; }
        }
    }
}
