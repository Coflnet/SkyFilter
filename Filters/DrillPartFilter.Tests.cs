using System.Collections.Generic;
using Coflnet.Sky.Core;
using NUnit.Framework;

namespace Coflnet.Sky.Filter;

public class DrillPartFilterTests
{
    [Test]
    public void DrillPartEngineMatchesLegacyAndPrefixedFlatNbtKeys()
    {
        var filter = new DrillPartEngineFilter();
        var engine = new FilterEngine(new MockNbt());
        var args = new FilterArgs(new Dictionary<string, string>()
        {
            { "DrillPartEngine", "titanium_drill_engine" }
        }, false, engine);
        var expression = filter.GetExpression(args).Compile();

        Assert.That(expression(new SaveAuction
        {
            FlatenedNBT = new Dictionary<string, string>
            {
                { "drill_part_engine", "titanium_drill_engine" }
            }
        }), Is.True);

        Assert.That(expression(new SaveAuction
        {
            FlatenedNBT = new Dictionary<string, string>
            {
                { "engine.id", "TITANIUM_DRILL_ENGINE" }
            }
        }), Is.True);
    }

    private class MockNbt : INBT
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
            return name switch
            {
                "engine.id" => 1,
                "drill_part_engine" => 2,
                _ => 0
            };
        }

        public int GetValueId(short key, string value)
        {
            return 1;
        }
    }
}