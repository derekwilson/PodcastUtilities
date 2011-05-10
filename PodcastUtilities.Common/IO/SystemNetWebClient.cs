using System;
using System.IO;
using System.Net;

namespace PodcastUtilities.Common.IO
{
    /// <summary>
    /// provides access to the physical internet
    /// </summary>
    public class SystemNetWebClient : IWebClient, IDisposable
    {
        private readonly WebClient _webClient;

        /// <summary>
        /// provides access to the physical internet
        /// </summary>
        public SystemNetWebClient()
        {
            _webClient = new WebClient();
            // some servers can die without a user-agent
            _webClient.Headers.Add("User-Agent", "Mozilla/4.0+");
        }

        /// <summary>
        /// open a readable stream from the supplied url
        /// </summary>
        /// <param name="address">url</param>
        /// <returns>readable stream</returns>
        public Stream OpenRead(Uri address)
        {
            return _webClient.OpenRead(address);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _webClient.Dispose();
        }
    }
}