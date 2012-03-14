using System;
using System.Diagnostics;

namespace PodcastUtilities.Common.Platform
{
    /// <summary>
    /// abstract away the performace monitor implementation
    /// </summary>
    public interface IPerfmonCounterUtilities
    {
        /// <summary>
        /// Test for the existence of a catagory
        /// </summary>
        Boolean Exists(string categoryName);

        /// <summary>
        /// Create a catagory
        /// </summary>
        void Create(string categoryName, string categoryDescription, PerformanceCounterCategoryType categoryType, CounterCreationDataCollection counters);

        /// <summary>
        /// delete a catagory
        /// </summary>
        void Delete(string categoryName);
    }
}
