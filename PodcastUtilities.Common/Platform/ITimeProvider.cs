using System;

namespace PodcastUtilities.Common.Platform
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
