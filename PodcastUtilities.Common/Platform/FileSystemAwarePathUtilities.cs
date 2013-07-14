using System;
using System.IO;
using PodcastUtilities.Common.Platform.Mtp;

namespace PodcastUtilities.Common.Platform
{
    ///<summary>
    /// Implementation of <see cref="IPathUtilities"/> that works with MTP paths
    ///</summary>
    public class FileSystemAwarePathUtilities : IPathUtilities
    {
        ///<summary>
        /// Returns the absolute path for the supplied path, using the current directory and volume if path is not already an absolute path.
        ///</summary>
        ///<param name="path">The file or directory for which to obtain absolute path information</param>
        ///<returns>A string containing the fully qualified location of path, such as "C:\MyFile.txt".</returns>
        public string GetFullPath(string path)
        {
            //If the path is an MTP path it is by definition already an absolute path

            return (MtpPath.IsMtpPath(path) ? path : Path.GetFullPath(path));
        }

        ///<summary>
        /// Creates a uniquely named, zero-byte temporary file on disk and returns the full path of that file.
        /// (The same as <see cref="Path.GetTempFileName"/>)
        ///</summary>
        ///<returns>The full path of the temporary file.</returns>
        public string GetTempFileName()
        {
            return Path.GetTempFileName();
        }
    }
}