using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace PodcastUtilities.AndroidLogic.Exceptions
{
    /// <summary>
    /// a non fatal exception that has a message
    /// </summary>
    [Serializable]
    [ComVisible(true)]
    [StructLayout(LayoutKind.Sequential)]
    public class NonFatalMessageException : System.Exception
    {
        /// <summary>
        /// a NonFatalMessageException
        /// </summary>
        public NonFatalMessageException()
            : base()
        { }

        /// <summary>
        /// a NonFatalMessageException
        /// </summary>
        public NonFatalMessageException(string message)
            : base(message)
        { }

        /// <summary>
        /// a NonFatalMessageException
        /// </summary>
        public NonFatalMessageException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}