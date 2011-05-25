using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PodcastUtilities.Common.IO
{
    /// <summary>
    /// abstract the date time for testing
    /// </summary>
    public interface ITimeProvider
    {
        /// <summary>
        /// get the current time
        /// </summary>
        DateTime Now { get; }
        /// <summary>
        /// get the current time - in utc/gmt
        /// </summary>
        DateTime UtcNow { get; }
    }
}
