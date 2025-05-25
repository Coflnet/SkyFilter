using System.Collections.Generic;
using Coflnet.Sky.Core;
using NUnit.Framework;

namespace Coflnet.Sky.Filter
{
    public class StarFilterTests
    {
        StarsFilter filter = new();
        [Test]
        public void None()
        {
            Assert.That(CreateForValue("none")(new SaveAuction()
            {
                NBTLookup = new NBTLookup[] { new NBTLookup(2, 1) }
            }), Is.False);
            Assert.That(CreateForValue("none")(new SaveAuction()
            {
                NBTLookup = new NBTLookup[0]
            }), Is.True);
        }
        [Test]
        public void One()
        {
            Assert.That(CreateForValue("1")(new SaveAuction()
            {
                NBTLookup = new NBTLookup[] { new NBTLookup(2, 1) }
            }), Is.True);
            Assert.That(CreateForValue("1")(new SaveAuction()
            {
                NBTLookup = new NBTLookup[] { new NBTLookup(2, 2) }
            }), Is.False);
        }
        [Test]
        public void BiggerOne()
        {
            Assert.That(CreateForValue(">1")(new SaveAuction()
            {
                NBTLookup = new NBTLookup[] { new NBTLookup(2, 2) }
            }), Is.True);
            Assert.That(CreateForValue(">1")(new SaveAuction()
            {
                NBTLookup = new NBTLookup[] { new NBTLookup(2, 9) }
            }), Is.True);
            Assert.That(CreateForValue(">1")(new SaveAuction()
            {
                NBTLookup = new NBTLookup[] { new NBTLookup(2, 1) }
            }), Is.False);
        }
        [Test]
        public void Range()
        {
            Assert.That(CreateForValue("1-9")(new SaveAuction()
            {
                NBTLookup = new NBTLookup[] { new NBTLookup(2, 2) }
            }), Is.True);
            Assert.That(CreateForValue("8-9")(new SaveAuction()
            {
                NBTLookup = new NBTLookup[] { new NBTLookup(2, 9) }
            }), Is.True);
            Assert.That(CreateForValue("4-8")(new SaveAuction()
            {
                NBTLookup = new NBTLookup[] { new NBTLookup(2, 1) }
            }), Is.False);
        }

        private System.Func<SaveAuction, bool> CreateForValue(string value)
        {
            var engine = new FilterEngine(new MockNbt());
            var args = new FilterArgs(new System.Collections.Generic.Dictionary<string, string>() { { "Stars", value } }, true, engine);
            var exp = filter.GetExpression(args).Compile();
            return exp;
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

            public short GetKeyId(string name)
            {
                return 2;
            }

            public int GetValueId(short key, string value)
            {
                return 1;
            }
        }
    }
}

