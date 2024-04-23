using Coflnet.Sky.Core;
using NUnit.Framework;

namespace Coflnet.Sky.Filter
{
    public class PetLevelFilterTests
    {
        FilterArgs args;
        private SaveAuction sampleAuction;
        private PetLevelFilter instance;

        [SetUp]
        public void Setup()
        {
            instance = new PetLevelFilter();
            args = new FilterArgs(new System.Collections.Generic.Dictionary<string, string>() { { "PetLevel", "5" } }, false);
            sampleAuction = new Core.SaveAuction()
            {
                ItemName = "[Lvl 33] TestPet",
                NBTLookup = new NBTLookup[] { new(2, 33) },
                FlatenedNBT = new () { { "candyUsed", "0" } },
            };
            NBT.Instance = new MockNbt();
        }
        [Test]
        public void ParseOnNonDB()
        {
            var selector = instance.GetSelector(args);
            var value = selector.Compile().Invoke(sampleAuction);
            Assert.That(33,Is.EqualTo(value));
        }
        [Test]
        public void ParseNonPet()
        {
            var selector = instance.GetSelector(args);
            sampleAuction.ItemName = "no pet level";
            var value = selector.Compile().Invoke(sampleAuction);
            Assert.That(-1,Is.EqualTo(value));
        }
        [Test]
        public void ParseRangePet()
        {
            args = new FilterArgs(new () { { "PetLevel", "1-99" } }, false);
            var selector = instance.GetExpression(args);
            var value = selector.Compile().Invoke(sampleAuction);
            Assert.That(value);
        }
        [Test]
        public void GoldenDragonAbove()
        {
            args = new FilterArgs(new System.Collections.Generic.Dictionary<string, string>() { { "Rarity", "Legendary" }, { "PetLevel", ">199" } }, true);
            sampleAuction.NBTLookup = new NBTLookup[] { new(2, 255_840_365) };
            sampleAuction.Tag = "PET_GOLDEN_DRAGON";
            var selector = instance.GetExpression(args);
            var value = selector.Compile().Invoke(sampleAuction);
            Assert.That(value);
        }
        [Test]
        public void GoldenDragonRange()
        {
            args = new FilterArgs(new System.Collections.Generic.Dictionary<string, string>() { { "Rarity", "Legendary" }, { "PetLevel", "175-175" } }, true);
            sampleAuction.NBTLookup = new NBTLookup[] { new(2, 164_223_663) };
            sampleAuction.Tag = "PET_GOLDEN_DRAGON";
            var selector = instance.GetExpression(args);

            var value = selector.Compile().Invoke(sampleAuction);
            Assert.That(value);
        }
        [Test]
        public void GoldenDragon101()
        {
            args = new FilterArgs(new System.Collections.Generic.Dictionary<string, string>() { { "Rarity", "Legendary" }, { "PetLevel", "101" } }, true);
            sampleAuction.NBTLookup = new NBTLookup[] { new(2, 25_353_257) };
            sampleAuction.Tag = "PET_GOLDEN_DRAGON";
            var selector = instance.GetExpression(args);
            var value = selector.Compile().Invoke(sampleAuction);
            Assert.That(value);
        }

        [Test]
        public void TigerCapsAt100()
        {
            args = new FilterArgs(new System.Collections.Generic.Dictionary<string, string>() { { "Rarity", "Legendary" }, { "PetLevel", "100" } }, true);
            sampleAuction.NBTLookup = new NBTLookup[] { new(2, 164_223_663) };
            sampleAuction.Tag = "PET_TIGER";
            var selector = instance.GetExpression(args);
            var value = selector.Compile().Invoke(sampleAuction);
            Assert.That(value);
        }

        [Test]
        public void LevelOneMatchesNoExp()
        {
            args = new FilterArgs(new System.Collections.Generic.Dictionary<string, string>() { { "Rarity", "Legendary" }, { "PetLevel", "1" } }, true);
            sampleAuction.NBTLookup = new NBTLookup[] { new(2, 0) };
            sampleAuction.Tag = "PET_TIGER";
            sampleAuction.Tier = Tier.LEGENDARY;
            var selector = instance.GetExpression(args);
            var value = selector.Compile().Invoke(sampleAuction);
            Assert.That(value, selector.ToString());
        }
    }
}
