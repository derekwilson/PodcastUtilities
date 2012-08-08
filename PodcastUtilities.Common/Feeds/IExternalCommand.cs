namespace PodcastUtilities.Common.Feeds
{
    /// <summary>
    /// an external command
    /// </summary>
    public interface IExternalCommand
    {
        /// <summary>
        /// the command
        /// </summary>
        string Command { get; set; }
        /// <summary>
        /// the arguments
        /// </summary>
        string Arguments { get; set; }
        /// <summary>
        /// the current working dir
        /// </summary>
        string WorkingDirectory { get; set; }
    }
}