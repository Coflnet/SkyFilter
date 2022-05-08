using System;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter
{
    public class EmanKillsFilter : NBTNumberFilter
    {
        public override Func<Coflnet.Sky.Items.Client.Model.Item, bool> IsApplicable => a
            => a.Tag.StartsWith("FINAL_DESTINATION_");
        protected override string PropName => "eman_kills";
    }
}

