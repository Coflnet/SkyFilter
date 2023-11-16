using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Coflnet.Sky.Core;
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
            var filters = new Dictionary<string, string>() { { "Enchantment", "critical" }, { "EnchantLvl", "6" }, { "Rarity", "EPIC" } };
            var successCount = 0;
            var matcher = engine.GetMatcher(filters);
            for (int i = 0; i < 3000; i++)
            {
                if (matcher(auction))
                    successCount++;
            }
            Assert.AreEqual(3000, successCount);
        }
        /// <summary>
        /// Enum sometimes favours one over the other, both need to work
        /// hypixel internally calls the enchant reiterate but displays duplex
        /// </summary>
        [Test]
        public void DuplexRemap()
        {
            SaveAuction auction = NewAuction();
            var engine = new FilterEngine();
            var filters = new Dictionary<string, string>() { { "ultimate_duplex", "1" }, { "ultimate_reiterate", "1" }};
            var matcher = engine.GetMatcher(filters);
            auction.Enchantments.Add(new Enchantment(Enchantment.EnchantmentType.ultimate_duplex, 1));
            Assert.IsTrue(matcher(auction));
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
            for (int i = 0; i < 3000; i++)
            {
                if (!matcher(auction))
                    successCount++;
            }
            Assert.AreEqual(3000, successCount);
        }

        private static SaveAuction NewAuction()
        {
            return new SaveAuction()
            {
                Enchantments = new System.Collections.Generic.List<Enchantment>() { new Enchantment(Enchantment.EnchantmentType.critical, 6) },
                Tier = Tier.EPIC,
                NBTLookup = new NBTLookup[0],
                FlatenedNBT = new Dictionary<string, string>()
            };
        }
    }
}
