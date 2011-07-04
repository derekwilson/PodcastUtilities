using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PodcastUtilities.Common
{
    /// <summary>
    /// information on the download progress
    /// </summary>
    public class DownloadProgressEventArgs : EventArgs
    {
        /// <summary>
        /// percentage of the download that is complete
        /// </summary>
        public int ProgressPercentage { get; set; }

        /// <summary>
        /// number of bytes that have been downloaded
        /// </summary>
        public long BytesReceived { get; set; }

        /// <summary>
        /// total number of bytes we are to download
        /// </summary>
        public long TotalBytesToReceive { get; set; }

        /// <summary>
        /// state that was passed to the downloader
        /// </summary>
        public object UserState { get; set; }
    }
}
