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
    [FilterDescription("Exact unlocked slots")]
    public class UnlockedSlotsMatchFilter : NBTFilter
    {
        protected override string PropName => "unlocked_slots";

        protected override long GetValueLong(string stringValue, short key)
        {
            // the default behaviour is to jsonserialize objects (including arrays) 
            // so they are stored as json in the db and the filter needs to mirror that to find matches 
            // see https://github.com/Coflnet/SkyFilter/issues/60
            var parts = stringValue.Split(',');
            return base.GetValueLong(JsonConvert.SerializeObject(parts), key);
        }
    }
    [FilterDescription("Amount of unlocked slots")]
    public class UnlockedSlotsFilter : NumberFilter
    {
        private static Dictionary<int, List<long>> Values;
        public UnlockedSlotsFilter()
        {

        }
        private static DateTime allUnlocked = new DateTime(2021, 9, 4);

        private void LoadLookup()
        {
            if (Values != null)
                return;
            Values = new Dictionary<int, List<long>>();
            Task.Run(async () =>
            {
                for (int i = 0; i < 100; i++)
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
                        await Task.Delay(TimeSpan.FromSeconds(5 * i));
                    }

            });

            for (int i = 0; i < 80; i++)
            {
                if (Values.Count > 4)
                    return;
                Task.Delay(50).Wait();
            }
            Values = null;
        }

        public virtual async Task<List<NBTValue>> LoadOptions()
        {
            using (var db = new HypixelContext())
            {
                return await db.NBTValues.Where(v => v.KeyId == db.NBTKeys.Where(k => k.Slug == "unlocked_slots").Select(k => k.Id).First()).ToListAsync();
            }
        }

        public override Func<Coflnet.Sky.Items.Client.Model.Item, bool> IsApplicable => i => i.Modifiers.Any(m => m.Slug == "unlocked_slots");

        public override IEnumerable<object> Options => new string[] { "0", "5" };

        public override Expression<Func<IDbItem, long>> GetSelector(FilterArgs args)
        {
            var argsLower = int.Parse(args.Filters[this.GetType().Name.Replace("Filter", "")].Split('-').Last().Trim('<', '>'));
            if (argsLower == 0)
                argsLower = 1;
            if (!args.TargetsDB)
                return a => (a as SaveAuction).ItemCreatedAt < allUnlocked ? argsLower : (a as SaveAuction).FlatenedNBT.Where(n => n.Key == "unlocked_slots").Select(n => n.Value.Count(x => x == ',') + 1).FirstOrDefault();
            LoadLookup();
            var keyId = NBT.Instance.GetKeyId("unlocked_slots");
            if (Values.Count < 5)
                throw new CoflnetException("not_loaded", "unlocked_slots not loaded yet, please wait a few seconds and try again");
            var oneIds = Values[1];
            var twoIds = Values[2];
            var threeIds = Values[3];
            var fourIds = Values[4];
            var fiveIds = Values[5];
            return a => (a as SaveAuction).ItemCreatedAt < allUnlocked ? argsLower : a.NBTLookup.Where(l => l.KeyId == keyId).Select(v =>
                oneIds.Contains(v.Value) ? 1
                : twoIds.Contains(v.Value) ? 2
                : threeIds.Contains(v.Value) ? 3
                : fourIds.Contains(v.Value) ? 5
                : fiveIds.Contains(v.Value) ? 5 : 6).FirstOrDefault();
        }

        public override Expression<Func<T, bool>> ExpressionMinMaxInstance<T>(Expression<Func<T, long>> selector, long min, long max, FilterArgs args)
        {
            if (args != null && !args.TargetsDB)
                return base.ExpressionMinMaxInstance(selector, min, max);
            var values = new HashSet<long>();
            for (long i = min; i < (max < 10 ? max : 10) + 1; i++)
            {
                if (Values.TryGetValue((int)i, out List<long> ids))
                    foreach (var item in ids.ToList())
                    {
                        values.Add(item);
                    }
            }
            var keyId = NBT.Instance.GetKeyId("unlocked_slots");
            if (values.Count() == 0)
                return a => !(a as SaveAuction).NBTLookup.Where(l => l.KeyId == keyId).Any() && (a as SaveAuction).ItemCreatedAt > allUnlocked;
            return a => (a as SaveAuction).NBTLookup.Where(l => l.KeyId == keyId && values.Contains(l.Value)).Any() || (a as SaveAuction).ItemCreatedAt < allUnlocked;
        }
    }
}
