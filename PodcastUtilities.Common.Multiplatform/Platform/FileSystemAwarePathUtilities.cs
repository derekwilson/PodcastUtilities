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