using System.Diagnostics;
using PodcastUtilities.Common.Perfmon;

namespace PodcastUtilities.Common.Platform
{
    /// <summary>
    /// system implementation
    /// </summary>
    public class SystemPerfmonCounterCreationDataProvider : IPerfmonCounterCreationDataProvider
    {
        /// <summary>
        /// construct a DTO to for use in the creation process
        /// </summary>
        public CounterCreationData GetCounter(string counterName, string counterDescription, PerformanceCounterType type)
        {
            return new CounterCreationData
                       {CounterName = counterName, CounterHelp = counterDescription, CounterType = type};
        }
    }
}
