using hypixel;
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
            Assert.IsFalse(CreateForValue("none")(new hypixel.SaveAuction()
            {
                NBTLookup = new System.Collections.Generic.List<NBTLookup>() { new NBTLookup(2, 1) }
            }));
            Assert.IsTrue(CreateForValue("none")(new hypixel.SaveAuction()
            {
                NBTLookup = new System.Collections.Generic.List<NBTLookup>()
            }));
        }
        [Test]
        public void One()
        {
            NBT.Instance = new MockNbt();
            Assert.IsTrue(CreateForValue("1")(new hypixel.SaveAuction()
            {
                NBTLookup = new System.Collections.Generic.List<NBTLookup>() { new NBTLookup(2, 1) }
            }));
            Assert.IsFalse(CreateForValue("1")(new hypixel.SaveAuction()
            {
                NBTLookup = new System.Collections.Generic.List<NBTLookup>() { new NBTLookup(2, 2) }
            }));
        }
        [Test]
        public void BiggerOne()
        {
            NBT.Instance = new MockNbt();
            Assert.IsTrue(CreateForValue(">1")(new hypixel.SaveAuction()
            {
                NBTLookup = new System.Collections.Generic.List<NBTLookup>() { new NBTLookup(2, 2) }
            }));
            Assert.IsTrue(CreateForValue(">1")(new hypixel.SaveAuction()
            {
                NBTLookup = new System.Collections.Generic.List<NBTLookup>() { new NBTLookup(2, 9) }
            }));
            Assert.IsFalse(CreateForValue(">1")(new hypixel.SaveAuction()
            {
                NBTLookup = new System.Collections.Generic.List<NBTLookup>() { new NBTLookup(2, 1) }
            }));
        }
        [Test]
        public void Range()
        {
            NBT.Instance = new MockNbt();
            Assert.IsTrue(CreateForValue("1-9")(new hypixel.SaveAuction()
            {
                NBTLookup = new System.Collections.Generic.List<NBTLookup>() { new NBTLookup(2, 2) }
            }));
            Assert.IsTrue(CreateForValue("8-9")(new hypixel.SaveAuction()
            {
                NBTLookup = new System.Collections.Generic.List<NBTLookup>() { new NBTLookup(2, 9) }
            }));
            Assert.IsFalse(CreateForValue("4-8")(new hypixel.SaveAuction()
            {
                NBTLookup = new System.Collections.Generic.List<NBTLookup>() { new NBTLookup(2, 1) }
            }));
        }

        private System.Func<SaveAuction, bool> CreateForValue(string value)
        {
            var args = new FilterArgs(new System.Collections.Generic.Dictionary<string, string>() { { "Stars", value } }, false);
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

