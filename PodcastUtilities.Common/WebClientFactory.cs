using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PodcastUtilities.Common.Platform;

namespace PodcastUtilities.Common
{
    /// <summary>
    /// factory class to generate a new web client
    /// </summary>
    public class WebClientFactory : IWebClientFactory
    {
        /// <summary>
        /// generate a new web client - do not forget to dispose it
        /// </summary>
        /// <returns>a web client</returns>
        public IWebClient GetWebClient()
        {
            return new SystemNetWebClient();
        }
    }
}
