using System;
using System.Diagnostics;

namespace PodcastUtilities.Common.Platform
{
    /// <summary>
    /// provides access to a system perfmon counter
    /// </summary>
    public class SystemPerfmonCounter : IPerfmonCounter
    {
        private readonly PerformanceCounter _counter;

        /// <summary>
        /// construct a counter
        /// </summary>
        public SystemPerfmonCounter(string catagory, string name)
        {
            try
            {
                _counter = new PerformanceCounter
                           {
                               CategoryName = catagory,
                               CounterName = name,
                               MachineName = ".",
                               ReadOnly = false,
                           };
            }
            catch
            {
                // counter not been created
            }
        }

        /// <summary>
        /// reset the counter
        /// </summary>
        public void Reset()
        {
            if (_counter == null)
            {
                return;
            }

            try
            {
                _counter.RawValue = 0;
            }
            catch
            {
            }
        }

        /// <summary>
        /// increment
        /// </summary>
        public void Increment()
        {
            if (_counter == null)
            {
                return;
            }

            try
            {
                _counter.Increment();
            }
            catch
            {
            }
        }

        /// <summary>
        /// increment by an amount
        /// </summary>
        /// <param name="value"></param>
        public void IncrementBy(long value)
        {
            if (_counter == null)
            {
                return;
            }

            try
            {
                _counter.IncrementBy(value);
            }
            catch
            {
            }
        }

        /// <summary>
        /// decrement 
        /// </summary>
        public void Decrement()
        {
            if (_counter == null)
            {
                return;
            }

            try
            {
                _counter.Decrement();
            }
            catch
            {
            }
        }
    }
}