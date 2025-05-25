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
        FilterEngine engine;
        SaveAuction auction;
        [SetUp]
        public void Setup()
        {
            engine = new FilterEngine(new MockNbt());
            auction = NewAuction();
        }
        [Test]
        public void Enchantments()
        {
            var filters = new Dictionary<string, string>() { { "Enchantment", "critical" }, { "EnchantLvl", "6" }, { "Rarity", "EPIC" } };
            var successCount = 0;
            var matcher = engine.GetMatcher(filters);
            for (int i = 0; i < 3000; i++)
            {
                if (matcher(auction))
                    successCount++;
            }
            Assert.That(3000, Is.EqualTo(successCount));
        }
        [TestCase("critical", 6, true)]
        [TestCase("sharpness", 6, false)]
        [TestCase("critical", 5, false)]
        public void EnchantMatching(string enchant, int level, bool expected)
        {
            var filters = new Dictionary<string, string>() { { enchant,  level.ToString() } };
            var matcher = engine.GetMatcher(filters);
            Assert.That(matcher(auction), Is.EqualTo(expected));
        }
        /// <summary>
        /// Enum sometimes favours one over the other, both need to work
        /// hypixel internally calls the enchant reiterate but displays duplex
        /// </summary>
        [Test]
        public void DuplexRemap()
        {
            var filters = new Dictionary<string, string>() { { "ultimate_duplex", "1" }, { "ultimate_reiterate", "1" } };
            var matcher = engine.GetMatcher(filters);
            auction.Enchantments.Add(new Enchantment(Enchantment.EnchantmentType.ultimate_duplex, 1));
            Assert.That(matcher(auction));
        }
        [Test]
        public void Skins()
        {
            var filters = new Dictionary<string, string>() { { "DragonArmorSkin", "marika" }, { "Rarity", "EPIC" } };
            var successCount = 0;
            //ItemDetails.Instance.TagLookup["marika"] = 1;
            var matcher = engine.GetMatcher(filters);
            for (int i = 0; i < 3000; i++)
            {
                if (!matcher(auction))
                    successCount++;
            }
            Assert.That(3000, Is.EqualTo(successCount));
        }

        [Test]
        public void Test()
        {
            var result = engine.GetMatchExpression(new() { { "Candy", "none" }, { "PetSkin", "any" } }, false).Compile();
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Invoke(new SaveAuction() { FlatenedNBT = new() { { "candyUsed", "0" }, { "skin", "something" } }, Tag = "PET_test" }), Is.True);
            Assert.That(result.Invoke(new SaveAuction() { FlatenedNBT = new() { { "candyUsed", "1" } }, Tag = "PET_test" }), Is.False);
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
