using System;

namespace PodcastUtilities.Common
{
    /// <summary>
    /// event mechanism for download update progress
    /// </summary>
    public interface IProgressUpdate
    {
        /// <summary>
        /// event for progress
        /// </summary>
        event EventHandler<ProgressEventArgs> ProgressUpdate;
    }
}