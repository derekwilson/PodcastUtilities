using System.Diagnostics;

namespace PodcastUtilities.Common.Platform
{
    /// <summary>
    /// used to isolate the main code from the physical system perfmon
    /// </summary>
    public interface IPerfmonCounterProvider
    {
        /// <summary>
        /// construct a DTO to for use in the creation process
        /// </summary>
        CounterCreationData GetCounter(string counterName, string counterDescription,
                                       PerformanceCounterType type);
    }
}
