using System;
using System.Runtime.Serialization;

namespace PodcastUtilities.AndroidLogic.Exceptions
{
    /// <summary>
    /// exceptions that relate to configuring the utilities
    /// </summary>
    [Serializable]
    public class ConfigurationException : System.Exception
    {
        /// <summary>
        /// a ControlFileValueFormatException
        /// </summary>
        public ConfigurationException()
            : base()
        { }

        /// <summary>
        /// a ControlFileValueFormatException
        /// </summary>
        public ConfigurationException(string message)
            : base(message)
        { }

        /// <summary>
        /// a ControlFileValueFormatException
        /// </summary>
        public ConfigurationException(string message, Exception innerException)
            : base(message, innerException)
        { }

        /// <summary>
        /// a ControlFileValueFormatException
        /// </summary>
        protected ConfigurationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }

}