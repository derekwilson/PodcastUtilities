using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PodcastUtilities.Common
{
    /// <summary>
    /// This is the event arg status update
    /// </summary>
    public class StatusUpdateEventArgs : System.EventArgs
    {
        public enum Level
        {
            Status,
            Warning,
            Error,
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
        /// Get the message level
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
