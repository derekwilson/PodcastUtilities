using System;
using System.Runtime.Serialization;

namespace PodcastUtilities.Common.Exceptions
{
    /// <summary>
    /// exceptions that relate to downloads
    /// </summary>
    [Serializable]
    public class DownloaderException : System.Exception
    {
        /// <summary>
        /// a downloader exception
        /// </summary>
        public DownloaderException()
            : base()
        { }

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

        /// <summary>
        /// a downloader exception
        /// </summary>
        protected DownloaderException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        { }
    }
}
