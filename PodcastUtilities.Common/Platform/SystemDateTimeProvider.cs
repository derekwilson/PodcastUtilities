using System;

namespace PodcastUtilities.Common.Platform
{
    /// <summary>
    /// access the System.DateTime for 
    /// </summary>
    public class SystemDateTimeProvider : ITimeProvider
    {

        #region ITimeProvider Members

        /// <summary>
        /// get the current time
        /// </summary>
        public DateTime Now
        {
            get { return DateTime.Now; }
        }

        /// <summary>
        /// get the current time - in utc/gmt
        /// </summary>
        public DateTime UtcNow
        {
            get { return DateTime.UtcNow; }
        }

        #endregion
    }
}