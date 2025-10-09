using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace PodcastUtilities.AndroidLogic.Exceptions
{
    /// <summary>
    /// exceptions that relate to downloading feeds and episodes
    /// </summary>
    [Serializable]
    [ComVisible(true)]
    [StructLayout (LayoutKind.Sequential)]
    public class DownloadException : System.Exception
    {
        /// <summary>
        /// a DownloadException
        /// </summary>
        public DownloadException()
            : base()
        { }

        /// <summary>
        /// a DownloadException
        /// </summary>
        public DownloadException(string message)
            : base(message)
        { }

        /// <summary>
        /// a DownloadException
        /// </summary>
        public DownloadException(string message, Exception innerException)
            : base(message, innerException)
        { }

        /// <summary>
        /// a DownloadException
        /// </summary>
        protected DownloadException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {}
    }
}