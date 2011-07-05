using System;
using System.ComponentModel;
using System.IO;
using System.Net;

namespace PodcastUtilities.Common.Platform
{
    /// <summary>
    /// methods to interact with the internet to isolate the main body of the code from the physical network
    /// </summary>
    public interface IWebClient : IProgressUpdate, IDisposable
    {
        /// <summary>
        /// open a readable stream from the supplied url
        /// </summary>
        /// <param name="address">url</param>
        /// <returns>readable stream</returns>
        Stream OpenRead(Uri address);

        ///<summary>
        /// event for completion
        ///</summary>
        event AsyncCompletedEventHandler DownloadFileCompleted;

        /// <summary>
        /// download a file async
        /// </summary>
        void DownloadFileAsync(Uri address, string fileName, object userToken);

        /// <summary>
        /// cancel an async operation
        /// </summary>
        void CancelAsync();
    }
}
