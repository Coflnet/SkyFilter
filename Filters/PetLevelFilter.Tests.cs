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
                ItemName = "[Lvl 33] TestPet"
            };
        }
        [Test]
        public void ParseOnNonDB()
        {
            var selector = instance.GetSelector(args);
            var value = selector.Compile().Invoke(sampleAuction);
            Assert.AreEqual(33, value);
        }
        [Test]
        public void ParseNonPet()
        {
            var selector = instance.GetSelector(args);
            sampleAuction.ItemName = "no pet level";
            var value = selector.Compile().Invoke(sampleAuction);
            Assert.AreEqual(-1, value);
        }
        [Test]
        public void ParseRangePet()
        {
            args = new FilterArgs(new System.Collections.Generic.Dictionary<string, string>() { { "PetLevel", "1-99" } }, false);
            var selector = instance.GetExpression(args);
            var value = selector.Compile().Invoke(sampleAuction);
            Assert.IsTrue(value);
        }
        [Test]
        public void GoldenDragonAbove()
        {
            args = new FilterArgs(new System.Collections.Generic.Dictionary<string, string>() { { "Rarity", "Legendary" }, { "PetLevel", ">199" } }, true);
            NBT.Instance = new MockNbt();
            sampleAuction.NBTLookup = new() { new(2, 255_840_365) };
            var selector = instance.GetExpression(args);
            var value = selector.Compile().Invoke(sampleAuction);
            Assert.IsTrue(value);
        }
        [Test]
        public void GoldenDragonRange()
        {
            args = new FilterArgs(new System.Collections.Generic.Dictionary<string, string>() { { "Rarity", "Legendary" }, { "PetLevel", "175-175" } }, true);
            NBT.Instance = new MockNbt();
            sampleAuction.NBTLookup = new() { new(2, 164_223_663) };
            var selector = instance.GetExpression(args);

            var value = selector.Compile().Invoke(sampleAuction);
            Assert.IsTrue(value);
        }
    }
}
