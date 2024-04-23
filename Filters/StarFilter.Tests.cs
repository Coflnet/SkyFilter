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
            NBT.Instance = new MockNbt();
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
            NBT.Instance = new MockNbt();
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
            NBT.Instance = new MockNbt();
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
            NBT.Instance = new MockNbt();
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
            var args = new FilterArgs(new System.Collections.Generic.Dictionary<string, string>() { { "Stars", value } }, true);
            var exp = filter.GetExpression(args).Compile();
            return exp;
        }

        class MockNbt : INBT
        {
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

