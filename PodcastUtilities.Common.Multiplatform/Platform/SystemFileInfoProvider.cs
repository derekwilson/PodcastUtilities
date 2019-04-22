using System;
using System.Collections.Generic;
using System.Text;

namespace PodcastUtilities.Common.Platform
{
    /// <summary>
    /// Provides the correct kind of file info object
    /// </summary>
    public class SystemFileInfoProvider : IFileInfoProvider
    {
        /// <summary>
        /// create an abstract file info object
        /// </summary>
        /// <param name="path">full path to the file</param>
        /// <returns>the file info</returns>
        public IFileInfo GetFileInfo(string path)
        {
            return new SystemFileInfo(new System.IO.FileInfo(path));
        }
    }
}
