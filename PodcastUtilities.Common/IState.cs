using System;

namespace PodcastUtilities.Common
{
    /// <summary>
    /// state object
    /// </summary>
    public interface IState
    {
        /// <summary>
        /// the latest publish date for a downloaded podcast
        /// </summary>
        DateTime DownloadHighTide { get; set; }

        /// <summary>
        /// persist the state
        /// </summary>
        void SaveState(string folder);
    }
}