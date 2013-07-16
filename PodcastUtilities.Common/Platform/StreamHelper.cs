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

namespace PodcastUtilities.Common.Platform
{
    /// <summary>
    /// Stream helper methods
    /// </summary>
    public class StreamHelper : IStreamHelper
    {
        private const int CopyBufferSize = 10000;

        ///<summary>
        /// Open a readable stream on a file
        ///</summary>
        ///<param name="path"></param>
        ///<returns></returns>
        public Stream OpenRead(string path)
        {
            return File.OpenRead(path);
        }

        ///<summary>
        /// Open a writeable stream on a file
        ///</summary>
        ///<param name="path"></param>
        ///<param name="allowOverwrite"></param>
        ///<returns></returns>
        public Stream OpenWrite(string path, bool allowOverwrite)
        {
            if (!allowOverwrite && File.Exists(path))
            {
                throw new IOException(String.Format("Cannot open file for writing as it already exists and allowOverwrite is false: {0}", path));
            }

            // Make sure directory exists
            var directory = Path.GetDirectoryName(path);
            if (directory != null)
            {
                Directory.CreateDirectory(directory);
            }

            return File.Create(path);
        }

        ///<summary>
        /// Copy from one stream to another
        ///</summary>
        ///<param name="source">The source stream, must be readable</param>
        ///<param name="destination">The destination stream, must be writeable</param>
        public void Copy(Stream source, Stream destination)
        {
            var buffer = new byte[CopyBufferSize];

            while (true)
            {
                var readSize = source.Read(buffer, 0, CopyBufferSize);
                if (readSize == 0)
                {
                    break;
                }

                destination.Write(buffer, 0, readSize);
            }

            destination.Flush();
        }
    }
}