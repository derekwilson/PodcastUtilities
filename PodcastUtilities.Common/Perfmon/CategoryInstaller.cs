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
