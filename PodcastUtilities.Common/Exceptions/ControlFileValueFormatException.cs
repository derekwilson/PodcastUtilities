using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace PodcastUtilities.Common.Exceptions
{
    /// <summary>
    /// exceptions that relate to the representation of values in the control file
    /// </summary>
    [Serializable]
    public class ControlFileValueFormatException : System.Exception
    {
        /// <summary>
        /// a ControlFileValueFormatException
        /// </summary>
        public ControlFileValueFormatException()
            : base()
        { }

        /// <summary>
        /// a ControlFileValueFormatException
        /// </summary>
        public ControlFileValueFormatException(string message)
            : base(message)
        { }

        /// <summary>
        /// a ControlFileValueFormatException
        /// </summary>
        public ControlFileValueFormatException(string message, Exception innerException)
            : base(message, innerException)
        { }

        /// <summary>
        /// a ControlFileValueFormatException
        /// </summary>
        protected ControlFileValueFormatException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
