using System;
using System.Linq;
using System.Linq.Expressions;
using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter
{
    public abstract class NBTItemFilter : NBTFilter
    {
        protected override long GetValueLong(string stringValue, short key)
        {
            return ItemDetails.Instance.GetItemIdForTag(stringValue.ToUpper());
        }
    }
}

