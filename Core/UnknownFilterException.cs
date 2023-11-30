using Coflnet.Sky.Core;

namespace Coflnet.Sky.Filter
{
    public class UnknownFilterException : CoflnetException
    {
        public UnknownFilterException(string filterName) : base("unknown_filter", $"The filter `{filterName}` is not known, please remove it")
        {
        }
    }
}
