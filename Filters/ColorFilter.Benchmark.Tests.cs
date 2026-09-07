using System;
using System.Diagnostics;
using System.Linq;
using Coflnet.Sky.Core;
using NUnit.Framework;

namespace Coflnet.Sky.Filter;

public class ColorFilterBenchmark
{
    [Test, Explicit("Small synthetic CPU benchmark; run with --filter FullyQualifiedName~ColorFilterBenchmark")]
    public void CompareMatchers()
    {
        const int count = 4096, passes = 50;
        var random = new Random(42);
        var examples = new[] { 0xAAAAAA, 0xAAAAAC, 0x1122FF, 0x1A2A3A };
        var auctions = Enumerable.Range(0, count).Select(i =>
        {
            var rgb = i % 2 == 0 ? random.Next(0x1000000) : examples[(i / 2) % examples.Length];
            return new SaveAuction
            {
                NBTLookup = new[] { new NBTLookup(2, unchecked(rgb << 8)) },
                FlatenedNBT = new() { { "color", $"{rgb >> 16}:{(rgb >> 8) & 255}:{rgb & 255}" }, { "other", "value" } }
            };
        }).ToArray();

        foreach (var targetsDb in new[] { false, true })
        foreach (var color in new[] { "AAAAAA", "_E_E_E", "pattern:AABBCC", "AAAAAA-3" })
        {
            var args = new FilterArgs(new() { { "Color", color } }, targetsDb, null) { NbtIntance = new MockNbt() };
            var matcher = new ColorFilter().GetExpression(args).Compile();
            int Run()
            {
                var matches = 0;
                for (var pass = 0; pass < passes; pass++)
                    foreach (var auction in auctions)
                        if (matcher(auction)) matches++;
                return matches;
            }

            // Exclude filter construction, compilation and warmup from the measurement.
            for (var warmup = 0; warmup < 3; warmup++) Run();
            var times = new double[7];
            var allocations = new double[7];
            var hits = 0;
            for (var round = 0; round < times.Length; round++)
            {
                var allocated = GC.GetAllocatedBytesForCurrentThread();
                var start = Stopwatch.GetTimestamp();
                hits = Run();
                times[round] = Stopwatch.GetElapsedTime(start).TotalNanoseconds / (count * passes);
                allocations[round] = (GC.GetAllocatedBytesForCurrentThread() - allocated) / (double)(count * passes);
            }
            Array.Sort(times);
            Array.Sort(allocations);
            NUnit.Framework.TestContext.Progress.WriteLine($"{(targetsDb ? "DB predicate (CPU only)" : "Live"),-24} {color,-16} {times[3]:F1} ns/item, {allocations[3]:F0} B/item, {hits} hits/{count * passes}");
        }
    }
}
