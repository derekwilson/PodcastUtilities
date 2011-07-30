using PodcastUtilities.Common.Platform;

namespace PodcastUtilities.Common
{
    /// <summary>
    /// generate a new web client
    /// </summary>
    public interface IWebClientFactory
    {
        /// <summary>
        /// generate a new web client - do not forget to dispose it
        /// </summary>
        /// <returns>a web client</returns>
        IWebClient GetWebClient();
    }
}