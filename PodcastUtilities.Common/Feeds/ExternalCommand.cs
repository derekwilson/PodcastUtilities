namespace PodcastUtilities.Common.Feeds
{
    /// <summary>
    /// an external command
    /// </summary>
    public class ExternalCommand : IExternalCommand
    {
        /// <summary>
        /// the command
        /// </summary>
        public string Command { get; set; }
        /// <summary>
        /// the arguments
        /// </summary>
        public string Arguments { get; set; }
        /// <summary>
        /// the current working dir
        /// </summary>
        public string WorkingDirectory { get; set; }
    }
}