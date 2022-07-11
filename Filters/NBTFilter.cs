using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Coflnet.Sky.Core;
using Coflnet.Sky.Items.Client.Model;

namespace Coflnet.Sky.Filter
{
    public abstract class NBTFilter : GeneralFilter
    {
        protected abstract string PropName { get; }

        public override Func<Item, bool> IsApplicable =>  a
            => a.Modifiers.Any(m=>m.Slug == PropName);

        public override FilterType FilterType => FilterType.Equal;

        public override IEnumerable<object> OptionsGet(OptionValues options)
        {
            return options.Options[PropName];
        }

        public override Expression<Func<SaveAuction, bool>> GetExpression(FilterArgs args)
        {
            var key = NBT.Instance.GetKeyId(PropName);
            var value = NBT.Instance.GetValueId(key, args.Get(this));
            return a => a.NBTLookup.Where(l => l.KeyId == key && l.Value == value).Any();
        }
    }
}

