using System;
using System.ComponentModel;
using System.IO;
using System.Net;

namespace PodcastUtilities.Common.Platform
{
    /// <summary>
    /// provides access to the physical internet
    /// </summary>
    public class SystemNetWebClient : IWebClient
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
        /// event for progress
        /// </summary>
        public event DownloadProgressChangedEventHandler DownloadProgressChanged
        {
            add { _webClient.DownloadProgressChanged += value; }
            remove { _webClient.DownloadProgressChanged -= value; }
        }

        ///<summary>
        /// event for completion
        ///</summary>
        public event AsyncCompletedEventHandler DownloadFileCompleted
        {
            add { _webClient.DownloadFileCompleted += value; }
            remove { _webClient.DownloadFileCompleted -= value; }
        }

        /// <summary>
        /// download a file async
        /// </summary>
        public void DownloadFileAsync(Uri address, string fileName, object userToken)
        {
            _webClient.DownloadFileAsync(address,fileName,userToken);
        }

        /// <summary>
        /// cancel an async operation
        /// </summary>
        public void CancelAsync()
        {
            _webClient.CancelAsync();
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