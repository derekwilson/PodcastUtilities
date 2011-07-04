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
        /// event for progress
        /// </summary>
        public event EventHandler<DownloadProgressEventArgs> ProgressUpdate;

        private void OnProgressUpdate(DownloadProgressEventArgs e)
        {
            if (ProgressUpdate != null)
                ProgressUpdate(this, e);
        }

        /// <summary>
        /// provides access to the physical internet
        /// </summary>
        public SystemNetWebClient()
        {
            _webClient = new WebClient();
            // some servers can die without a user-agent
            _webClient.Headers.Add("User-Agent", "Mozilla/4.0+");
            _webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(WebClientDownloadProgressChanged);
        }

        private void WebClientDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            // we need to repack the System.Net.DownloadProgressChangedEventArgs
            // into a PodcastUtilities.Common.DownloadProgressEventArgs
            // as the System.Net.DownloadProgressChangedEventArgs cannot be constructed for testing as it has an internat constructor

            var progress = new DownloadProgressEventArgs()
                               {
                                   ProgressPercentage = e.ProgressPercentage,
                                   BytesReceived = e.BytesReceived,
                                   TotalBytesToReceive = e.TotalBytesToReceive,
                                   UserState = e.UserState
                               };
            OnProgressUpdate(progress);
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