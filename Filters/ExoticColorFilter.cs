using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter
{
    public class ExoticColorFilter : ColorFilter
    {
        public override FilterType FilterType => FilterType.Equal;

        HashSet<string> FairyColors = new(){
            "330066", "4C0099", "660033", "660066", "6600CC", "7F00FF", "99004C", "990099", "9933FF", "B266FF",
            "CC0066", "CC00CC", "CC99FF", "E5CCFF", "FF007F", "FF00FF", "FF3399", "FF33FF", "FF66B2", "FF66FF",
            "FF99CC", "FF99FF", "FFCCE5", "FFCCFF"};
        public override IEnumerable<object> OptionsGet(OptionValues options)
        {
            var all = options.Options["color"]
                // the most common one is the default
                .Skip(1)
                .Select(dec => ToHex(dec))
                .Where(hex => !FairyColors.Contains(hex))
                // only the rarest 10 are of interest
                .Reverse().Take(10)
                .ToList();

            return all.Prepend($"Any:{string.Join(',', all)}");
        }

        public override Expression<Func<SaveAuction, bool>> GetExpression(FilterArgs args)
        {
            var key = NBT.Instance.GetKeyId("color");
            var stringVal = args.Get(this);
            var values = stringVal.Split(':').Last().Split(',').Select(hex=>FromHex(hex));

            return a => a.NBTLookup.Where(l => l.KeyId == key && values.Contains(l.Value)).Any();
        }
    }
}
