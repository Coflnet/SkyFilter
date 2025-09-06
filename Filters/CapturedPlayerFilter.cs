using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Coflnet.Sky.Core;
using Microsoft.EntityFrameworkCore;

namespace Coflnet.Sky.Filter;
public class CapturedPlayerFilter : GeneralFilter
{
    protected virtual string Propname => "captured_player";

    public override FilterType FilterType => FilterType.Equal | FilterType.TEXT | FilterType.PLAYER_WITH_RANK;
    public override IEnumerable<object> Options => new object[] { "", "" };

    public override Func<Items.Client.Model.Item, bool> IsApplicable => item
                => item?.Tag == "CAKE_SOUL";

    public override Expression<Func<IDbItem, bool>> GetExpression(FilterArgs args)
    {
        var key = args.NbtIntance.GetKeyId(Propname);
        var name = args.Get(this);
        if (string.IsNullOrWhiteSpace(name))
            return a => !a.NBTLookup.Where(l => l.KeyId == key).Any();
        if (!args.TargetsDB)
        {
            // If the user passed only a name (no bracketed rank part with "] "), match the plain name anywhere
            if (!name.Contains("]"))
            {
                var nameRegex = new Regex(Regex.Escape(name), RegexOptions.IgnoreCase);
                return a => (a as SaveAuction).FlatenedNBT.Where(n => n.Key == Propname).Select(n => n.Value).Any(v => nameRegex.IsMatch(v)) || nameRegex.IsMatch((a as SaveAuction).ItemName);
            }

            var parts = name.Split("] ");
            var playerName = ".*";
            if(parts.Length > 1)
                playerName = parts[1].Trim();
            var regex = new Regex(parts[0].Trim('[',']', '+') + ".*\\] " + playerName, RegexOptions.IgnoreCase);
            return a => (a as SaveAuction).FlatenedNBT.Where(n => n.Key == Propname).Select(n => n.Value).Any(v => regex.IsMatch(v)) || regex.IsMatch((a as SaveAuction).ItemName);
        }
        var val = FindValueId(key, name);
        return a => a.NBTLookup.Where(l => l.KeyId == key && l.Value == val).Any() || a.ItemName == name;
    }

    private long FindValueId(short key, string name)
    {
        using var db = new HypixelContext();
        var processed = "%" + name.Split("] ").Last().Trim();
        var value = db.NBTValues.Where(v => v.KeyId == key && EF.Functions.Like(v.Value, processed)).OrderByDescending(v => v.Id).FirstOrDefault();
        if (value == null)
            throw new CoflnetException("not_found", $"No player with name like '{name}' found");
        return value.Id;
    }
}
