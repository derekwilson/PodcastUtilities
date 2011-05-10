using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PodcastUtilities.Common.IO
{
    /// <summary>
    /// methods to interact with the internet to isolate the main body of the code from the physical network
    /// </summary>
    public interface IWebClient
    {
        /// <summary>
        /// open a readable stream from the supplied url
        /// </summary>
        /// <param name="address">url</param>
        /// <returns>readable stream</returns>
        Stream OpenRead(Uri address);
    }
}
