namespace PodcastUtilities.Common
{
    /// <summary>
    /// the type of the update
    /// </summary>
    public enum StatusUpdateLevel
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
}