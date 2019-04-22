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
#if NETFULL
            if (_enableCounters)
            {
                returnValue = new AverageCounter(catagory, name, totalName);
            }
#endif
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
#if NETFULL
            if (_enableCounters)
            {
                returnValue = new SystemPerfmonCounter(catagory, name);
            }
#endif
            if (returnValue == null)
            {
                return new NullCounter();
            }
            return returnValue;
        }
    }
}
