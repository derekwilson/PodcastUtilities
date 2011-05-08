using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PodcastUtilities.Common
{
    /// <summary>
    /// This is the event argument that is passed whenever a status update is fired
    /// </summary>
    public class StatusUpdateEventArgs : System.EventArgs
    {
        /// <summary>
        /// the type of the update
        /// </summary>
        public enum Level
        {
            /// <summary>
            /// information to be displayed
            /// </summary>
            Status,
            /// <summary>
            /// a non critical warning
            /// </summary>
            Warning,
            /// <summary>
            /// a fatal error
            /// </summary>
            Error,
            /// <summary>
            /// extra information that may be useful
            /// </summary>
            Verbose
        }

        private readonly Level _level;
        private readonly string _message;

        /// <summary>
        /// Construct a new message event arg.
        /// </summary>
        public StatusUpdateEventArgs(Level level, string message)
        {
            _message = message;
            _level = level;
        }

        /// <summary>
        /// Get the type of the update
        /// </summary>
        public Level MessageLevel
        {
            get
            {
                return _level;
            }
        }
        /// <summary>
        /// Get the message text
        /// </summary>
        public string Message
        {
            get
            {
                return _message;
            }
        }
    }
}
