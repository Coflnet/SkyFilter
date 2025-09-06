using System.Collections.Generic;
using Coflnet.Sky.Core;
using NUnit.Framework;

namespace Coflnet.Sky.Filter
{
    public class CakeOwnerFilterTests
    {
        CakeOwnerFilter filter = new();

        [Test]
        public void MatchesFromFlatenedNBT()
        {
            var engine = new FilterEngine(new MockNbt());
            var args = new FilterArgs(new Dictionary<string, string>() { { "CakeOwner", "[YOUTUBE] im_a_squid_kid" } }, false, engine);
            var exp = filter.GetExpression(args).Compile();

            Assert.That(exp(new SaveAuction()
            {
                FlatenedNBT = new Dictionary<string, string>() { { "cake_owner", "§c[§fYOUTUBE§c] im_a_squid_kid" } },
                ItemName = "Some Item"
            }), Is.True);
        }

        [Test]
        public void MatchesFromItemName()
        {
            var engine = new FilterEngine(new MockNbt());
            var args = new FilterArgs(new Dictionary<string, string>() { { "CakeOwner", "[MVP+] oScarlet" } }, false, engine);
            var exp = filter.GetExpression(args).Compile();

            Assert.That(exp(new SaveAuction()
            {
                FlatenedNBT = new Dictionary<string, string>() { },
                ItemName = "§b[MVP§4+§b] oScarlet"
            }), Is.True);
        }

        [Test]
        public void MatchesVariousNames()
        {
            var engine = new FilterEngine(new MockNbt());
            var args1 = new FilterArgs(new Dictionary<string, string>() { { "CakeOwner", "[ADMIN] Minikloon" } }, false, engine);
            var args2 = new FilterArgs(new Dictionary<string, string>() { { "CakeOwner", "[MVP++] YT_OS_Broke" } }, false, engine);

            var exp1 = filter.GetExpression(args1).Compile();
            var exp2 = filter.GetExpression(args2).Compile();

            Assert.That(exp1(new SaveAuction() { FlatenedNBT = new Dictionary<string, string>() { { "cake_owner", "§c[ADMIN] Minikloon" } } }), Is.True);
            Assert.That(exp2(new SaveAuction() { FlatenedNBT = new Dictionary<string, string>() { { "cake_owner", "§6[MVP§f++§6] YT_OS_Broke" } } }), Is.True);
        }

        [Test]
        public void MatchesRankOnly()
        {
            var engine = new FilterEngine(new MockNbt());
            var args = new FilterArgs(new Dictionary<string, string>() { { "CakeOwner", "[ADMIN]" } }, false, engine);
            var exp = filter.GetExpression(args).Compile();

            Assert.That(exp(new SaveAuction() { FlatenedNBT = new Dictionary<string, string>() { { "cake_owner", "§c[ADMIN] Minikloon" } } }), Is.True);
        }

        [Test]
        public void MatchesNameOnly()
        {
            var engine = new FilterEngine(new MockNbt());
            var args = new FilterArgs(new Dictionary<string, string>() { { "CakeOwner", "Minikloon" } }, false, engine);
            var exp = filter.GetExpression(args).Compile();

            Assert.That(exp(new SaveAuction() { FlatenedNBT = new Dictionary<string, string>() { { "cake_owner", "§c[ADMIN] Minikloon" } } }), Is.True);
            Assert.That(exp(new SaveAuction() { FlatenedNBT = new Dictionary<string, string>() { }, ItemName = "A cake by Minikloon" }), Is.True);
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
                // return a distinct key id for "cake_owner"
                return (short)(name == "cake_owner" ? 42 : 0);
            }

            public int GetValueId(short key, string value)
            {
                return 1;
            }
        }
    }
}
