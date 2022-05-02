using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Coflnet.Sky.Core;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Coflnet.Sky.Filter
{
    public class UnlockedSlotsFilter : NumberFilter
    {
        private static Dictionary<int, List<long>> Values;
        public UnlockedSlotsFilter()
        {

        }

        private void LoadLookup()
        {
            if (Values != null)
                return;
            Values = new Dictionary<int, List<long>>();
            Task.Run(async () =>
            {
                try
                {
                    var values = await LoadOptions();

                    foreach (var item in values)
                    {
                        if (item.Value == null)
                            continue;
                        var count = item.Value.Count(x => x == ',') + 1;
                        if (!Values.TryGetValue(count, out List<long> ids))
                        {
                            ids = new List<long>();
                            Values[count] = ids;
                        }
                        ids.Add(item.Id);
                    }
                    Console.WriteLine("loaded unlocked slots ids");
                }
                catch (Exception e)
                {
                    dev.Logger.Instance.Error(e, "failed to load unlocked_slots");
                }

            });

            for (int i = 0; i < 30; i++)
            {
                if (Values.Count > 4)
                    return;
                Task.Delay(50).Wait();
            }
        }

        public virtual async Task<List<NBTValue>> LoadOptions()
        {
            using (var db = new HypixelContext())
            {
                return await db.NBTValues.Where(v => v.KeyId == db.NBTKeys.Where(k => k.Slug == "unlocked_slots").Select(k => k.Id).First()).ToListAsync();
            }
        }

        public override Func<DBItem, bool> IsApplicable => i => i.Tag.Contains("DIVAN") || i.Tag.Contains("SORROW");

        public override IEnumerable<object> Options => new string[]{"0","5"};

        public override Expression<Func<SaveAuction, long>> GetSelector(FilterArgs args)
        {
            LoadLookup();
            var keyId = NBT.Instance.GetKeyId("unlocked_slots");
            var oneIds = Values[1];
            var twoIds = Values[2];
            var threeIds = Values[3];
            var fourIds = Values[4];
            var fiveIds = Values[5];
            return a => a.NBTLookup.Where(l => l.KeyId == keyId).Select(v =>
                oneIds.Contains(v.Value) ? 1
                : twoIds.Contains(v.Value) ? 2
                : threeIds.Contains(v.Value) ? 3
                : fourIds.Contains(v.Value) ? 5
                : fiveIds.Contains(v.Value) ? 5 : 6).FirstOrDefault();
        }

        public override Expression<Func<T, bool>> ExpressionMinMaxInstance<T>(Expression<Func<T, long>> selector, long min, long max)
        {
            var values = new HashSet<long>();
            for (long i = min; i < (max < 10 ? max : 10) + 1; i++)
            {
                if (Values.TryGetValue((int)i, out List<long> ids))
                    foreach (var item in ids)
                    {
                        values.Add(item);
                    }
            }
            var keyId = NBT.Instance.GetKeyId("unlocked_slots");
            if (values.Count() == 0)
                return a => !(a as SaveAuction).NBTLookup.Where(l => l.KeyId == keyId).Any();
            return a => (a as SaveAuction).NBTLookup.Where(l => l.KeyId == keyId && values.Contains(l.Value)).Any();
        }
    }
}
