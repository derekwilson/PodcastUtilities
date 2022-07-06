#region License
// FreeBSD License
// Copyright (c) 2010 - 2013, Andrew Trevarrow and Derek Wilson
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
// Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
// 
// Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED 
// WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A 
// PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR
// ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
// TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
// POSSIBILITY OF SUCH DAMAGE.
#endregion
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
        private readonly Boolean _completed;
        private readonly Object _userState;

        /// <summary>
        /// Construct a new message event arg.
        /// </summary>
        public StatusUpdateEventArgs(StatusUpdateLevel level, string message, Boolean complete, Object state) : this(level, message, null, complete, state)
        {
        }

        /// <summary>
        /// Construct a new message event arg.
        /// </summary>
        public StatusUpdateEventArgs(StatusUpdateLevel level, string message, Exception exception, Boolean complete, Object state)
        {
            _message = message;
            _level = level;
            _completed = complete;
            _userState = state;
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

        /// <summary>
        /// user state that was passed to the task
        /// </summary>
        public object UserState
        {
            get
            {
                return _userState;
            }
        }

        /// <summary>
        /// has the task run to completion
        /// </summary>
        public Boolean IsTaskCompletedSuccessfully
        {
            get
            {
                return _completed;
            }
        }

    }
}
