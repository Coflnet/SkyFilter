using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Coflnet.Sky.Core;
using System.Linq;
using System.Text.RegularExpressions;

namespace Coflnet.Sky.Filter
{
    public class PetLevelFilter : NumberFilter
    {
        public override Func<Coflnet.Sky.Items.Client.Model.Item, bool> IsApplicable => PetFilter.IsPet;
        private static Regex nameRegex = new Regex(@"Lvl (\d{1,3})");
        private static List<long> xpPerlevel = new List<long>(){
            100,
            110,
            120,
            130,
            145,
            160,
            175,
            190,
            210,
            230,
            250,
            275,
            300,
            330,
            360,
            400,
            440,
            490,
            540,
            600,
            660,
            730,
            800,
            880,
            960,
            1050,
            1150,
            1260,
            1380,
            1510,
            1650,
            1800,
            1960,
            2130,
            2310,
            2500,
            2700,
            2920,
            3160,
            3420,
            3700,
            4000,
            4350,
            4750,
            5200,
            5700,
            6300,
            7000,
            7800,
            8700,
            9700,
            10800,
            12000,
            13300,
            14700,
            16200,
            17800,
            19500,
            21300,
            23200,
            25200,
            27400,
            29800,
            32400,
            35200,
            38200,
            41400,
            44800,
            48400,
            52200,
            56200,
            60400,
            64800,
            69400,
            74200,
            79200,
            84700,
            90700,
            97200,
            104200,
            111700,
            119700,
            128200,
            137200,
            146700,
            156700,
            167700,
            179700,
            192700,
            206700,
            221700,
            237700,
            254700,
            272700,
            291700,
            311700,
            333700,
            357700,
            383700,
            411700,
            441700,
            476700,
            516700,
            561700,
            611700,
            666700,
            726700,
            791700,
            861700,
            936700,
            1016700,
            1101700,
            1191700,
            1286700,
            1386700,
            1496700,
            1616700,
            1746700,
            1886700,
            0
        };
        public override Expression<Func<SaveAuction, long>> GetSelector(FilterArgs args)
        {
            if (ShoulParseFromName(args))
                return a => a.ItemName != null && nameRegex.IsMatch(a.ItemName) ? int.Parse(nameRegex.Match(a.ItemName).Groups[1].Value) : -1;
            var keyId = NBT.Instance.GetKeyId("exp");
            return a => a.NBTLookup.Where(a => a.KeyId == keyId).Select(a => a.Value).FirstOrDefault();
        }

        public override Expression<Func<SaveAuction, bool>> GetExpression(FilterArgs args)
        {
            if (new char[] { 'X', 'x', '_' }.Any(i => args.Get(this).Contains(i)) || !args.TryGet(new RarityFilter(), out _))
                if (new char[] { '>', '<', '-' }.Any(i => args.Get(this).Contains(i)))
                {
                    if (args.TargetsDB)
                        throw new CoflnetException("invalid_filter", "You need to select a rarity to use pet ranges");
                }
                else
                    return new PetLevelOldFilter().GetExpression(args);
            return base.GetExpression(args);
        }

        public override long GetLowerBound(FilterArgs args, long input)
        {
            if (ShoulParseFromName(args))
                return input;
            return XpForLevel(args, input - 1);
        }

        private static bool ShoulParseFromName(FilterArgs args)
        {
            return !args.TargetsDB && !args.TryGet(new RarityFilter(), out _);
        }

        public override long GetUpperBound(FilterArgs args, long input)
        {
            if (ShoulParseFromName(args))
                return input;
            if (input >= 100)
                return System.Int32.MaxValue;
            return XpForLevel(args, input);
        }

        private static long XpForLevel(FilterArgs args, long input)
        {
            var xp = 0L;
            if (!args.TryGet(new RarityFilter(), out string rarityString))
                rarityString = "";
            Enum.TryParse<Tier>(rarityString, true, out Tier rarity);
            var rarityBonus = rarity switch
            {
                Tier.UNCOMMON => 6,
                Tier.RARE => 11,
                Tier.EPIC => 16,
                Tier.LEGENDARY => 20,
                Tier.MYTHIC => 20,
                Tier.DIVINE => 20,
                _ => 0
            };
            var itterations = input + rarityBonus;
            if (itterations < 0)
                return 0;
            if (itterations >= xpPerlevel.Count)
                itterations = xpPerlevel.Count - 1;
            for (int i = 0; i < itterations; i++)
            {
                xp += xpPerlevel[i];
            }
            return xp;
        }
    }
}
