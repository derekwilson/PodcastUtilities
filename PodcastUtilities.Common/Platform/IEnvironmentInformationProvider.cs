using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PodcastUtilities.Common.Platform
{
    /// <summary>
    /// provides information on the application environment
    /// </summary>
    public interface IEnvironmentInformationProvider
    {
        /// <summary>
        /// gett the directory that the currently executing application was loaded from
        /// </summary>
        /// <returns></returns>
        IDirectoryInfo GetCurrentApplicationDirectory();
    }
}
