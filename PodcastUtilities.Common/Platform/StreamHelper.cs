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