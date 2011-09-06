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
        private readonly StatusUpdateLevel _level;
        private readonly string _message;
        private readonly Exception _excpetion;

        /// <summary>
        /// Construct a new message event arg.
        /// </summary>
        public StatusUpdateEventArgs(StatusUpdateLevel level, string message)
        {
            _message = message;
            _level = level;
            _excpetion = null;
        }

        /// <summary>
        /// Construct a new message event arg.
        /// </summary>
        public StatusUpdateEventArgs(StatusUpdateLevel level, string message, Exception exception)
        {
            _message = message;
            _level = level;
            _excpetion = exception;
        }

        /// <summary>
        /// Get the type of the update
        /// </summary>
        public StatusUpdateLevel MessageLevel
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
        /// <summary>
        /// Get the message text
        /// </summary>
        public Exception Exception
        {
            get
            {
                return _excpetion;
            }
        }
    }
}
