using System;

namespace Coflnet.Sky.Filter
{
    [Flags]
    public enum FilterType
    {
        /// <summary>
        /// Only one element is allowed
        /// </summary>
        Equal = 1,
        HIGHER = 2,
        LOWER = 4,
        /// <summary>
        /// Datetimestamp
        /// </summary>
        DATE = 8,
        /// <summary>
        /// Only numerical input allowed
        /// </summary>
        NUMERICAL = 16,
        /// <summary>
        /// The command is a slider or input field
        /// </summary>
        RANGE = 32,
        TEXT = 64,
        /// <summary>
        /// Tells the UI to use a simple interface
        /// </summary>
        SIMPLE = 128,
        BOOLEAN = 256
    }
}
