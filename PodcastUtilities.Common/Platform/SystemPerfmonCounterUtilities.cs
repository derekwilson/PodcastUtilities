using System.Diagnostics;

namespace PodcastUtilities.Common.Platform
{
    /// <summary>
    /// system implementation of the performance counters
    /// </summary>
    public class SystemPerfmonCounterUtilities : IPerfmonCounterUtilities
    {
        /// <summary>
        /// Test for the existence of a catagory
        /// </summary>
        public bool Exists(string categoryName)
        {
            return PerformanceCounterCategory.Exists(categoryName);
        }

        /// <summary>
        /// Create a catagory
        /// </summary>
        public void Create(string categoryName, string categoryDescription, PerformanceCounterCategoryType categoryType, CounterCreationDataCollection counters)
        {
            PerformanceCounterCategory.Create(categoryName, categoryDescription, categoryType, counters);
        }

        /// <summary>
        /// delete a catagory
        /// </summary>
        public void Delete(string categoryName)
        {
            PerformanceCounterCategory.Delete(categoryName);
        }

    }
}
