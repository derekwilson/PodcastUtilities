#region License
// FreeBSD License
// Copyright (c) 2010 - 2013, Andrew Trevarrow and Derek Wilson
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
// Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
// 
// Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED 
// WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A 
// PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR
// ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
// TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
// POSSIBILITY OF SUCH DAMAGE.
#endregion
using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;

namespace PodcastUtilities.Common.Platform
{
    /// <summary>
    /// provides access to the physical internet
    /// </summary>
    public sealed class SystemNetWebClient : IWebClient
    {
        private readonly WebClient _webClient;

        /// <summary>
        /// event for progress
        /// </summary>
        public event EventHandler<ProgressEventArgs> ProgressUpdate;

        private void OnProgressUpdate(ProgressEventArgs e)
        {
            ProgressUpdate?.Invoke(this, e);
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
            // as the System.Net.DownloadProgressChangedEventArgs cannot be constructed for testing as it has an internal constructor

            var progress = new ProgressEventArgs()
                               {
                                   ProgressPercentage = e.ProgressPercentage,
                                   ItemsProcessed = e.BytesReceived,
                                   TotalItemsToProcess = e.TotalBytesToReceive,
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