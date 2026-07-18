using System;
using System.Collections.Generic;
using System.Linq;
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
            private Dictionary<string, string> flatenedNbt;

            /// <summary>
            /// Carries the pre-flattened nbt (e.g. the item color) sent as plain json, since the
            /// binary <see cref="SaveAuction.NbtData"/> can not be transported over json.
            /// <para>
            /// <see cref="Newtonsoft.Json.ObjectCreationHandling.Replace"/> is required: the inherited
            /// getter returns a fresh empty dictionary when nothing is set yet, so with the default
            /// (Auto) handling Newtonsoft would populate that throwaway instance and never invoke the
            /// setter, silently dropping the incoming values.
            /// </para>
            /// </summary>
            [Newtonsoft.Json.JsonProperty("flatNbt", ObjectCreationHandling = Newtonsoft.Json.ObjectCreationHandling.Replace)]
            public override Dictionary<string, string> FlatenedNBT
            {
                get => flatenedNbt ?? base.FlatenedNBT;
                set => flatenedNbt = value;
            }
        }
    }
}
