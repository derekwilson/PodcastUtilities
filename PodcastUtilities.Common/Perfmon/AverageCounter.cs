#region License
// FreeBSD License
// Copyright (c) 2010 - 2013, Andrew Trevarrow and Derek Wilson
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
// Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
// 
// Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED 
// WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A 
// PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR
// ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
// TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
// POSSIBILITY OF SUCH DAMAGE.
#endregion
using System;
using System.Diagnostics;
using PodcastUtilities.Common.Platform;

namespace PodcastUtilities.Common.Perfmon
{
    /// <summary>
    /// counter to measure the average time for an event
    /// </summary>
    public class AverageCounter : IAverageCounter
    {
        private readonly IPerfmonCounter _averageTime;
        private readonly IPerfmonCounter _averageTimeBase;
        private readonly IPerfmonCounter _totalCounter;

        /// <summary>
        /// make an average time counter
        /// </summary>
        public AverageCounter(string counterCategory, string averageCounterName) : this(counterCategory,averageCounterName,null)
        {
        }

        /// <summary>
        /// make an average time counter and a total number of events counter
        /// </summary>
        public AverageCounter(string counterCategory, string averageCounterName, string totalCounterName)
        {
            _averageTime = new SystemPerfmonCounter(counterCategory, averageCounterName);
            _averageTimeBase = new SystemPerfmonCounter(counterCategory, averageCounterName + "Base");
            if (!string.IsNullOrEmpty(totalCounterName))
            {
                _totalCounter = new SystemPerfmonCounter(counterCategory, totalCounterName);
            }
        }

        /// <summary>
        /// reset the counter
        /// </summary>
        public void Reset()
        {
            if (_averageTime != null && _averageTimeBase != null)
            {
                _averageTime.Reset();
                _averageTimeBase.Reset();
            }
            if (_totalCounter != null)
            {
                _totalCounter.Reset();
            }
        }

        /// <summary>
        /// register the time for a single event and increments the total counter
        /// </summary>
        public void RegisterTime(Stopwatch timer)
        {
            if (_averageTime != null && _averageTimeBase != null)
            {
                _averageTime.IncrementBy(timer.ElapsedMilliseconds);
                _averageTimeBase.Increment();
            }
            if (_totalCounter != null)
            {
                _totalCounter.Increment();
            }
        }

        /// <summary>
        /// register the value to be recorded against a single event and then increments the total counter by the value as well
        /// </summary>
        public void RegisterValue(long value)
        {
            if (_averageTime != null && _averageTimeBase != null)
            {
                _averageTime.IncrementBy(value);
                _averageTimeBase.Increment();
            }
            if (_totalCounter != null)
            {
                _totalCounter.IncrementBy(value);
            }
        }
    }
}
