using Coflnet.Sky.Core;
using NUnit.Framework;

namespace Coflnet.Sky.Filter
{
    public class PetLevelFilterTests
    {
        FilterArgs args;
        private SaveAuction sampleAuction;

        [SetUp]
        public void Setup()
        {
            args = new FilterArgs(new System.Collections.Generic.Dictionary<string, string>() { { "PetLevel", "5" } }, false);
            sampleAuction = new Core.SaveAuction()
            {
                ItemName = "[Lvl 33] TestPet"
            };
        }
        [Test]
        public void ParseOnNonDB()
        {
            var instance = new PetLevelFilter();

            var selector = instance.GetSelector(args);
            var value = selector.Compile().Invoke(sampleAuction);
            Assert.AreEqual(33, value);
        }
    }
}