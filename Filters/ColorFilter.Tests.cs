using System;
using System.Collections.Generic;
using System.Linq;
using Coflnet.Sky.Core;
using Microsoft.EntityFrameworkCore;
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

        [TestCase("_E_E_E", "1A2A3A", true)]
        [TestCase("_E_E_E", "1E2E3F", false)]
        [TestCase("pattern:AABBCC", "1122FF", true)]
        [TestCase("pattern:AABBCC", "FFCC88", true)]
        [TestCase("pattern:AABBCC", "1222FF", false)]
        [TestCase("pattern:AABBCC", "1123FF", false)]
        [TestCase("pattern:AABBCC", "1122FE", false)]
        [TestCase("pattern:AAAAAA", "000000", true)]
        [TestCase("pattern:AAAAAA", "FFFFFF", true)]
        [TestCase("pattern:AAAAAA", "FFFFFE", false)]
        [TestCase("pattern:ABCABC", "ABCABC", true)]
        [TestCase("pattern:ABCABC", "D07D07", true)]
        [TestCase("pattern:ABCABC", "D07D08", false)]
        [TestCase("pattern:ABCABC", "ABCAAC", false)]
        [TestCase("PATTERN:aabbcc", "000000", true)]
        [TestCase("______", "123456", true)]
        [TestCase("AABBCC", "1122FF", false)]
        [TestCase("AABBCC", "AABBCC", true)]
        [TestCase("#FFFFFF", "FFFFFF", true)]
        [TestCase("112233, FFFFFF", "FFFFFF", true)]
        [TestCase("255:255:255", "FFFFFF", true)]
        [TestCase("AAAAAA-3", "AAAAAC", true)]
        [TestCase("AAAAAA-3", "AAAAA7", true)]
        [TestCase("AAAAAA-3", "AAAAAD", true)]
        [TestCase("AAAAAA-3", "AAAAAE", false)]
        [TestCase("AAAAAA-3", "AAAAA6", false)]
        [TestCase("#aaaaaa-0", "AAAAAA", true)]
        [TestCase("AAAAAA-0", "AAAAAB", false)]
        [TestCase("000000-3", "000000", true)]
        [TestCase("000000-3", "FFFFFF", false)]
        [TestCase("FFFFFF-3", "FFFFFF", true)]
        [TestCase("FFFFFF-3", "000000", false)]
        [TestCase("7FFFFF-3", "800002", false)]
        [TestCase("7FFFFF-3", "800003", false)]
        [TestCase("800000-3", "7FFFFD", false)]
        [TestCase("800000-3", "7FFFFC", false)]
        [TestCase("AAAAAA-10", "AAAAB4", true)]
        [TestCase("000000-765", "FFFFFF", true)]
        [TestCase("000000-764", "FFFFFF", false)]
        [TestCase("F2DF11-11", "F0DE09", true)]
        [TestCase("F2DF11-10", "F0DE09", false)]
        [TestCase("AAAAAA-3", "ABABAB", true)]
        [TestCase("AAAAAA-3", "ABABAC", false)]
        [TestCase("800000-3", "810101", true)]
        public void DatabaseAndLiveAgree(string pattern, string hex, bool expected)
        {
            var rgb = Convert.ToInt32(hex, 16);
            var auction = new SaveAuction
            {
                NBTLookup = new[] { new NBTLookup(2, new ColorFilter().FromHex(hex)) },
                FlatenedNBT = new() { { "color", $"{rgb >> 16}:{(rgb >> 8) & 255}:{rgb & 255}" } }
            };
            foreach (var targetsDb in new[] { true, false })
                Assert.That(Matcher(pattern, targetsDb)(auction), Is.EqualTo(expected), $"TargetsDB={targetsDb}");
        }

        [TestCase("pattern:AABBCC")]
        [TestCase("_E_E_E")]
        [TestCase("______")]
        [TestCase("AAAAAA-3")]
        public void RequiresColor(string pattern)
        {
            var auction = new SaveAuction { NBTLookup = new[] { new NBTLookup(3, 0) }, FlatenedNBT = new() { { "other", "0:0:0" } } };
            Assert.That(Matcher(pattern, true)(auction), Is.False);
            var live = Matcher(pattern, false);
            Assert.That(live(auction), Is.False);
            auction.FlatenedNBT = null;
            Assert.That(live(auction), Is.False);
        }

        [TestCase("pattern:ABC")]
        [TestCase("pattern:AABBCCD")]
        [TestCase("pattern:AA!BCC")]
        [TestCase("_E_E_")]
        [TestCase("AAAAAA--3")]
        [TestCase("AAAAAA-766")]
        [TestCase("AAAAAA-999999999999999")]
        [TestCase("AAAAAA-x")]
        [TestCase("AAAAA-3")]
        [TestCase("GGGGGG-3")]
        public void InvalidPatternsAreRejected(string pattern)
        {
            foreach (var targetsDb in new[] { true, false })
                Assert.Throws<CoflnetException>(() => Matcher(pattern, targetsDb));
        }

        [TestCase("pattern:AABBCC")]
        [TestCase("_E_E_E")]
        public void PatternsTranslateToSql(string pattern)
        {
            var options = new DbContextOptionsBuilder<HypixelContext>()
                .UseMySql("server=localhost;database=test", new MariaDbServerVersion("10.3")).Options;
            using var context = new HypixelContext(options);
            var engine = new FilterEngine(new MockNbt());
            var query = context.Auctions.Where(engine.GetMatchExpression(new() { { "Color", pattern }, { "ItemId", "123" } }, true));
            var sql = query.ToQueryString();
            Assert.That(sql, Does.Contain("EXISTS").And.Contain("&").And.Contain("*"));
            Assert.That(sql, Does.Contain("ItemId"));
        }

        [TestCase("AAAAAA-3")]
        [TestCase("7FFFFF-3")]
        public void ToleranceTranslatesToSql(string color)
        {
            var options = new DbContextOptionsBuilder<HypixelContext>()
                .UseMySql("server=localhost;database=test", new MariaDbServerVersion("10.3")).Options;
            using var context = new HypixelContext(options);
            var engine = new FilterEngine(new MockNbt());
            var sql = context.Auctions.Where(engine.GetMatchExpression(new() { { "Color", color } }, true)).ToQueryString();
            Assert.That(sql, Does.Contain("EXISTS").And.Contain("ABS(").And.Contain("<=").And.Contain("&"));
        }

        private static Func<IDbItem, bool> Matcher(string pattern, bool targetsDb)
        {
            var args = new FilterArgs(new() { { "Color", pattern } }, targetsDb, null) { NbtIntance = new MockNbt() };
            return new ColorFilter().GetExpression(args).Compile();
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
