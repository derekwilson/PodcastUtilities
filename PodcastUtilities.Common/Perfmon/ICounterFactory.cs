using PodcastUtilities.Common.Perfmon;

namespace PodcastUtilities.Common.Platform
{
    /// <summary>
    /// factory class for creating a counter
    /// </summary>
    public interface ICounterFactory
    {
        /// <summary>
        /// swicth the counters on or off
        /// </summary>
        void EnableCounters(bool enableCounters);
        /// <summary>
        /// create an average counter with an optional total counter
        /// </summary>
        IAverageCounter CreateAverageCounter(string catagory, string name, string totalName);
        /// <summary>
        /// create a counter
        /// </summary>
        IPerfmonCounter CreateCounter(string catagory, string name);
    }
}