using System;
using System.Collections.Generic;
using Coflnet.Sky.Core;
using NUnit.Framework;

namespace Coflnet.Sky.Filter
{
    public class ColorFilterTests
    {
        [Test]
        public void Matches()
        {
            Test("37b042", 934298112);
            Test("fcf3ff", -51118336, "Necrons chestplate");
            Test("e7413c", -415155200, "Necrons chestplate 2");
        }

        private static void Test(string stringVersion, int code, string message = null)
        {
            var filter = new ColorFilter();
            var args = new FilterArgs(new System.Collections.Generic.Dictionary<string, string>() { { "Color", stringVersion } }, true, null);
            args.NbtIntance = new MockNbt();

            var exp = filter.GetExpression(args);
            var matches = exp.Compile()(new SaveAuction() { NBTLookup = new [] { new NBTLookup(2, code) } });
            if (!matches)
            {
                var conv = filter.FromHex(stringVersion);
                Console.WriteLine();
                Console.WriteLine(conv.ToString("X") + conv);
                Console.WriteLine(code.ToString("X") + code);
            }
            Assert.That(matches, Is.True, message);
        }

    }
    class MockNbt : INBT
    {
        public int Value { get; set; }
        public MockNbt(int value = 1)
        {
            Value = value;
        }
        public short GetKeyId(string name)
        {
            return 2;
        }

        public int GetValueId(short key, string value)
        {
            return Value;
        }

        public NBTLookup[] CreateLookup(string auctionTag, Dictionary<string, object> data, List<KeyValuePair<string, object>> flatList = null)
        {
            throw new NotImplementedException();
        }

        public NBTLookup[] CreateLookup(SaveAuction auction)
        {
            throw new NotImplementedException();
        }

        public long GetItemIdForSkin(string name)
        {
            throw new NotImplementedException();
        }
    }
}
