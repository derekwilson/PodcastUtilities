using System.Diagnostics;

namespace PodcastUtilities.Common.Perfmon
{
    /// <summary>
    /// update or refresh a performance category
    /// </summary>
    public interface ICategoryInstaller
    {
        /// <summary>
        /// refresh the catagory (creating if needed) and then update the catagory to contain all the counters that have been added to the installer
        /// </summary>
        CategoryInstallerRefeshResult RefreshCatagoryWithCounters(string categoryName, string categoryDescription);
        /// <summary>
        /// remove this category
        /// </summary>
        CategoryInstallerRefeshResult DeleteCatagory(string categoryName);
        /// <summary>
        /// add a counter to the installer, this counter can then be installed by calling the RefreshCatagoryWithCounters method
        /// </summary>
        void AddCounter(string counterName, string counterHelp, PerformanceCounterType counterType);
    }
}
