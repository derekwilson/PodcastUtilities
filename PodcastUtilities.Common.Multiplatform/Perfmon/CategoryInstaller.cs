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
using System.Diagnostics;
using PodcastUtilities.Common.Platform;

namespace PodcastUtilities.Common.Perfmon
{
    /// <summary>
    /// mechanism used to install categories
    /// </summary>
    public class CategoryInstaller : ICategoryInstaller
    {
        /// <summary>
        /// the category we will create the common counters in
        /// </summary>
        public const string PodcastUtilitiesCommonCounterCategory = "PodcastUtilties:Common";

        /// <summary>
        /// counter name
        /// </summary>
        public const string AverageTimeToDownload = "AverageTimeToDownloadInMs";
        /// <summary>
        /// counter name
        /// </summary>
        public const string NumberOfDownloads = "TotalNumberOfDownloads";
        
        /// <summary>
        /// counter name
        /// </summary>
        public const string AverageMBDownload = "AverageSizeOfADownloadInMB";
        /// <summary>
        /// counter name
        /// </summary>
        public const string SizeOfDownloads = "TotalSizeOfDownloads";

        private IPerfmonCounterCreationDataProvider _counterCreator;
        private IPerfmonCounterUtilities _performanceCounterCategoryProxy;
        private CounterCreationDataCollection _counters;

        /// <summary>
        /// create an installer
        /// </summary>
        public CategoryInstaller(
            IPerfmonCounterUtilities performanceCounterCategoryProxy,
            IPerfmonCounterCreationDataProvider counterFactory)
        {
            _performanceCounterCategoryProxy = performanceCounterCategoryProxy;
            _counterCreator = counterFactory;
            _counters = new CounterCreationDataCollection();
        }

        /// <summary>
        /// remove this category
        /// </summary>
        public CategoryInstallerRefeshResult DeleteCatagory(string categoryName)
        {
            CategoryInstallerRefeshResult result = CategoryInstallerRefeshResult.CatagoryDeleted;

            _performanceCounterCategoryProxy.Delete(categoryName);

            return result;
        }

        /// <summary>
        /// refresh the catagory (creating if needed) and then update the catagory to contain all the counters that have been added to the installer
        /// </summary>
        public CategoryInstallerRefeshResult RefreshCatagoryWithCounters(string categoryName, string categoryDescription)
        {
            CategoryInstallerRefeshResult result = CategoryInstallerRefeshResult.CatagoryCreated;

            if (_performanceCounterCategoryProxy.Exists(categoryName))
            {
                _performanceCounterCategoryProxy.Delete(categoryName);
                result = CategoryInstallerRefeshResult.CatagoryUpdated;
            }

            _performanceCounterCategoryProxy.Create(categoryName,
                                                    categoryDescription,
                                                    PerformanceCounterCategoryType.SingleInstance, _counters);
            return result;
        }

        /// <summary>
        /// add a counter to the installer, this counter can then be installed by calling the RefreshCatagoryWithCounters method
        /// </summary>
        public void AddCounter(string counterName, string counterHelp, PerformanceCounterType counterType)
        {
            var counter = _counterCreator.GetCounter(counterName, counterHelp, counterType);

            if (!_counters.Contains(counter))
            {
                _counters.Add(counter);
            }
        }
    }
}
