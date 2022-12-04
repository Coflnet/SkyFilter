using System.Linq;
using Coflnet.Sky.Core;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;


namespace Coflnet.Sky.Filter
{
    public class NumberFilterTests
    {
        [Test]
        public void HighestBidAmount()
        {
            var filter = new HighestBidFilter();
            var full = filter.GetExpression(new FilterArgs(new System.Collections.Generic.Dictionary<string, string>()
            {
                { "HighestBid", "0m-500m"}
            }, true)).Compile();

            Assert.IsTrue(full(new SaveAuction() { HighestBidAmount = 50 }));
            Assert.IsFalse(full(new SaveAuction() { HighestBidAmount = 5_000_000_000 }));
        }
    }
}
