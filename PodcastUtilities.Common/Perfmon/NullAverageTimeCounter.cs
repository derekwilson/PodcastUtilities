using System;
using System.Diagnostics;

namespace PodcastUtilities.Common.Perfmon
{
    /// <summary>
    /// a counter that does nothing
    /// </summary>
    public class NullAverageTimeCounter : IAverageCounter
    {
        /// <summary>
        /// reset the counter
        /// </summary>
        public void Reset()
        {
        }

        /// <summary>
        /// register the time for a single event
        /// </summary>
        /// <param name="timer"></param>
        public void RegisterTime(Stopwatch timer)
        {
        }

        /// <summary>
        /// regirster the value to be recorded against a single event
        /// </summary>
        /// <param name="value"></param>
        public void RegisterValue(long value)
        {
        }
    }
}