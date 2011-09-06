using System;
using System.Runtime.Serialization;

namespace PodcastUtilities.Common.Exceptions
{
    /// <summary>
    /// exceptions that relate the structure of a feed
    /// </summary>
    [Serializable]
    public class FeedStructureException : System.Exception
    {
        /// <summary>
        /// a downloader exception
        /// </summary>
        public FeedStructureException()
            : base()
        { }

        /// <summary>
        /// a downloader exception
        /// </summary>
        public FeedStructureException(string message)
            : base(message)
        { }

        /// <summary>
        /// a downloader exception
        /// </summary>
        public FeedStructureException(string message, Exception innerException)
            : base(message, innerException)
        { }

        /// <summary>
        /// a downloader exception
        /// </summary>
        protected FeedStructureException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}