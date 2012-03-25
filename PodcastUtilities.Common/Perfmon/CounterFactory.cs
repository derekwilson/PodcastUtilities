using System;
using PodcastUtilities.Common.Perfmon;

namespace PodcastUtilities.Common.Platform
{
    /// <summary>
    /// factory class to produce a counter
    /// </summary>
    public class CounterFactory : ICounterFactory
    {
        private bool _enableCounters;

        /// <summary>
        /// live constructor
        /// </summary>
        public CounterFactory()
        {
            // on by default
            _enableCounters = true;
        }

        /// <summary>
        /// swicth the counters on or off
        /// </summary>
        public void EnableCounters(bool enableCounters)
        {
            _enableCounters = enableCounters;
        }

        /// <summary>
        /// create an average counter with an optional total counter
        /// </summary>
        public IAverageCounter CreateAverageCounter(string catagory, string name, string totalName)
        {
            IAverageCounter returnValue = null;
            if (_enableCounters)
            {
                returnValue = new AverageCounter(catagory, name, totalName);
            }
            if (returnValue == null)
            {
                return new NullAverageTimeCounter();
            }
            return returnValue;
        }

        /// <summary>
        /// create a counter
        /// </summary>
        public IPerfmonCounter CreateCounter(string catagory, string name)
        {
            IPerfmonCounter returnValue = null;
            if (_enableCounters)
            {
                returnValue = new SystemPerfmonCounter(catagory, name);
            }
            if (returnValue == null)
            {
                return new NullCounter();
            }
            return returnValue;
        }
    }
}
