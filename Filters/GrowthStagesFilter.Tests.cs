using System.Collections.Generic;
using Coflnet.Sky.Core;
using NUnit.Framework;

namespace Coflnet.Sky.Filter
{
    public class GrowthStagesFilterTests
    {
        private GrowthStagesFilter instance;
        private FilterEngine filterEngine = new(new MockNbt());

        [SetUp]
        public void Setup()
        {
            instance = new GrowthStagesFilter();
        }

        [Test]
        public void LessThanOneMatchesSmallSeconds()
        {
            var args = new FilterArgs(new Dictionary<string, string>() { { "GrowthStages", "<1" } }, true, filterEngine);
            var selector = instance.GetExpression(args).Compile();
            var auction = new SaveAuction() { NBTLookup = new NBTLookup[] { new(1, 580) } };

            Assert.That(selector(auction));
        }

        [Test]
        public void GreaterThanOneMatches200000()
        {
            var args = new FilterArgs(new Dictionary<string, string>() { { "GrowthStages", ">=1" } }, true, filterEngine);
            var selector = instance.GetExpression(args).Compile();
            var auction = new SaveAuction() { NBTLookup = new NBTLookup[] { new(1, 200000) } };

            Assert.That(selector(auction));
        }

        [Test]
        public void TwoMatches380000()
        {
            var args = new FilterArgs(new Dictionary<string, string>() { { "GrowthStages", "2" } }, true, filterEngine);
            var selector = instance.GetExpression(args).Compile();
            var auction = new SaveAuction() { NBTLookup = new NBTLookup[] { new(1, 380000) } };

            Assert.That(selector(auction));
        }

        class MockNbt : INBT
        {
            public NBTLookup[] CreateLookup(string auctionTag, Dictionary<string, object> data, List<KeyValuePair<string, object>> flatList = null)
            {
                throw new System.NotImplementedException();
            }

            public NBTLookup[] CreateLookup(SaveAuction auction)
            {
                throw new System.NotImplementedException();
            }

            public long GetItemIdForSkin(string name)
            {
                throw new System.NotImplementedException();
            }

            public short GetKeyId(string name)
            {
                return 1;
            }

            public int GetValueId(short key, string value)
            {
                return 1;
            }
        }
    }
}
