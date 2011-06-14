using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PodcastUtilities.Common
{
    /// <summary>
    /// exceptions that relate to downloads
    /// </summary>
    public class DownloaderException : System.Exception
    {
        /// <summary>
        /// a downloader exception
        /// </summary>
        public DownloaderException(string message)
            : base(message)
        { }

        /// <summary>
        /// a downloader exception
        /// </summary>
        public DownloaderException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
