using System;
using System.Runtime.Serialization;

namespace PodcastUtilities.Common.Exceptions
{
    /// <summary>
    /// exceptions that relate to downloads
    /// </summary>
    [Serializable]
    public class XmlStructureException : System.Exception
    {
        /// <summary>
        /// a downloader exception
        /// </summary>
        public XmlStructureException()
            : base()
        { }

        /// <summary>
        /// a downloader exception
        /// </summary>
        public XmlStructureException(string message)
            : base(message)
        { }

        /// <summary>
        /// a downloader exception
        /// </summary>
        public XmlStructureException(string message, Exception innerException)
            : base(message, innerException)
        { }

        /// <summary>
        /// a downloader exception
        /// </summary>
        protected XmlStructureException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        { }
    }
}