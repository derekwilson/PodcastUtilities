using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PodcastUtilities.Common
{
    /// <summary>
    /// information on the progress of a task
    /// </summary>
    public class ProgressEventArgs : EventArgs
    {
        /// <summary>
        /// percentage of the download that is complete
        /// </summary>
        public int ProgressPercentage { get; set; }

        /// <summary>
        /// number of items processed - for example number of bytes that have been downloaded
        /// </summary>
        public long ItemsProcessed { get; set; }

        /// <summary>
        /// total number of items to process - for example total number of bytes we are to download
        /// </summary>
        public long TotalItemsToProcess { get; set; }

        /// <summary>
        /// user state that was passed to the task
        /// </summary>
        public object UserState { get; set; }
    }
}
