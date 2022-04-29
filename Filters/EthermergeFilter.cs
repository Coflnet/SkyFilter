using System;
using System.Linq;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter
{
    public class EthermergeFilter : BoolNbtFilter
    {
        public override Func<DBItem, bool> IsApplicable => item
                    => new string[] { "ASPECT_OF_THE_END", "ASPECT_OF_THE_VOID" }.Contains(item?.Tag);

        public override string Key => "ethermerge";

    }
}
