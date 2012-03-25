using System.Diagnostics;

namespace PodcastUtilities.Common.Perfmon
{
    /// <summary>
    /// a performance counter for measuring the average lenth of time it takes for an event
    /// </summary>
    public interface IAverageCounter
    {
        /// <summary>
        /// reset the counter
        /// </summary>
        void Reset();

        /// <summary>
        /// register the time for a single event and increments the total counter
        /// </summary>
        void RegisterTime(Stopwatch timer);

        /// <summary>
        /// register the value to be recorded against a single event and then increments the total counter by the value as well
        /// </summary>
        void RegisterValue(long value);
    }
}