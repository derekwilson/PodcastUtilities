using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PodcastUtilities.Common
{
    /// <summary>
    /// event mechanism for an update in terms of progress or warning/error message
    /// </summary>
    public interface IStatusUpdate
    {
        /// <summary>
        /// fire an event for an update in terms of progress or warning/error message
        /// </summary>
        event EventHandler<StatusUpdateEventArgs> StatusUpdate;
    }
}
