using System;

namespace PodcastUtilities.Common
{
    /// <summary>
    /// event mechanism for download update progress
    /// </summary>
    public interface IDownloadProgressUpdate
    {
        /// <summary>
        /// event for progress
        /// </summary>
        event EventHandler<DownloadProgressEventArgs> ProgressUpdate;
    }
}