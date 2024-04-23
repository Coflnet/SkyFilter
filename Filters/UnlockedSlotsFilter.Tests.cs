using System.Collections.Generic;
using System.Threading.Tasks;
using Coflnet.Sky.Core;
using Coflnet.Sky.Filter.Tests;
using NUnit.Framework;

namespace Coflnet.Sky.Filter
{
    public class UnlockedSlotsFilterTests
    {
        [Test]
        public void Simple()
        {
            var instance = new UnlockedSlotsFilterMock();
            NBT.Instance = new MockNbt();
            var exp = instance.GetExpression(new FilterArgs(new Dictionary<string, string>() { { "UnlockedSlotsMock", "1" } }, true));
            var result = exp.Compile().Invoke(new SaveAuction() { NBTLookup = new[] { new NBTLookup(2, 5) } });
            Assert.That(result);
        }

        [Test]
        public void NoDb()
        {
            var instance = new UnlockedSlotsFilter();
            NBT.Instance = new MockNbt();
            var exp = instance.GetExpression(new FilterArgs(new Dictionary<string, string>() { { "UnlockedSlots", "1" } }, false));
            var result = exp.Compile().Invoke(new SaveAuction() { FlatenedNBT = new() { { "unlocked_slots", "TOPAZ" } } });
            Assert.That(result);
        }
    }
    public class UnlockedSlotsFilterMock : UnlockedSlotsFilter
    {
        public override async Task<List<NBTValue>> LoadOptions()
        {
            await Task.Delay(1);
            return new List<NBTValue>() {
                new NBTValue(2, "TOPAZ") { Id = 5 },
                new NBTValue(2, "TOPAZ,AMBER") { Id = 6 },
                new NBTValue(2, "TOPAZ,AMBER,jerrald") { Id = 7 },
                new NBTValue(2, "TOPAZ,AMBER,jerrald,mark") { Id = 8 },
                new NBTValue(2, "TOPAZ,AMBER,jerrald,mark,kevin") { Id = 9 } };

        }
    }
}
