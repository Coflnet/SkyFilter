using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using hypixel;
using NUnit.Framework;

namespace Coflnet.Sky.Filter.Tests
{
    public class FilterEngineTests
    {
        [Test]
        public void Enchantments()
        {
            SaveAuction auction = NewAuction();
            var engine = new FilterEngine();
            var filters = new Dictionary<string, string>() { { "Enchantment", "critical" }, {"EnchantLvl", "6"}, {"Rarity", "EPIC"} };
            var successCount = 0;
            var matcher = engine.GetMatcher(filters);
            var stopWatch = Stopwatch.StartNew();
            for (int i = 0; i < 3000; i++)
            {
                if(matcher(auction))
                    successCount++;
            }
            Assert.AreEqual(3000, successCount);
            Assert.LessOrEqual(stopWatch.ElapsedMilliseconds, 10);
        }
        [Test]
        public void Skins()
        {
            SaveAuction auction = NewAuction();
            var engine = new FilterEngine();
            var filters = new Dictionary<string, string>() { { "DragonArmorSkin", "marika" }, { "Rarity", "EPIC" } };
            var successCount = 0;
            NBT.Instance = new MockNbt();
            ItemDetails.Instance.TagLookup["marika"] = 1;
            var matcher = engine.GetMatcher(filters);
            var stopWatch = Stopwatch.StartNew();
            for (int i = 0; i < 3000; i++)
            {
                if (!matcher(auction))
                    successCount++;
            }
            Assert.AreEqual(3000, successCount);
            Assert.LessOrEqual(stopWatch.ElapsedMilliseconds, 10);
        }

        private static SaveAuction NewAuction()
        {
            return new SaveAuction()
            {
                Enchantments = new System.Collections.Generic.List<Enchantment>() { new Enchantment(Enchantment.EnchantmentType.critical, 6) },
                Tier = Tier.EPIC,
                NBTLookup = new List<NBTLookup>(),
                FlatenedNBT = new Dictionary<string, string>()
            };
        }

        class MockNbt : INBT
        {
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
