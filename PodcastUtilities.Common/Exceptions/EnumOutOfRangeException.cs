using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace PodcastUtilities.Common.Exceptions
{
    /// <summary>
    /// exceptions that relate enums containing unexpected or out of range values
    /// </summary>
    [Serializable]
    public class EnumOutOfRangeException : System.Exception
    {
        /// <summary>
        /// a enum out of range exception
        /// </summary>
        public EnumOutOfRangeException()
            : base()
        { }

        /// <summary>
        /// a enum out of range exception
        /// </summary>
        public EnumOutOfRangeException(string message)
            : base(message)
        { }

        /// <summary>
        /// a enum out of range exception
        /// </summary>
        public EnumOutOfRangeException(string message, Exception innerException)
            : base(message, innerException)
        { }

        /// <summary>
        /// a enum out of range exception
        /// </summary>
        protected EnumOutOfRangeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
