﻿using System.IO;

namespace PodcastUtilities.Common.Platform
{
    ///<summary>
    /// Implementation of <see cref="IPathUtilities"/> that works with OS paths
    ///</summary>
    public class PathUtilities : IPathUtilities
    {
        ///<summary>
        /// Returns the absolute path for the supplied path, using the current directory and volume if path is not already an absolute path.
        ///</summary>
        ///<param name="path">The file or directory for which to obtain absolute path information</param>
        ///<returns>A string containing the fully qualified location of path, such as "C:\MyFile.txt".</returns>
        public string GetFullPath(string path)
        {
            return Path.GetFullPath(path);
        }

        /// <summary>
        /// get array of chars that are illegal in files in the current file system
        /// this could be incomplete thanks to Google's MediaStore shambles
        /// </summary>
        /// <returns>array of illegal chars</returns>
        public char[] GetInvalidFileNameChars()
        {
            return Path.GetInvalidFileNameChars();
        }

        /// <summary>
        /// get the char used by the current file system to seperate elements in the path
        /// </summary>
        /// <returns>path seperator char</returns>
        public char GetPathSeparator()
        {
            return Path.DirectorySeparatorChar;
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
