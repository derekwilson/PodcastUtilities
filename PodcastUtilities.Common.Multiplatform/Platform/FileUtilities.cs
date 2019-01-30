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
using System.IO;

namespace PodcastUtilities.Common.Platform
{
	/// <summary>
	/// utility methods to manipulate files in the physical file system
	/// this class abstracts away the file system from the main body of code
	/// </summary>
	public class FileUtilities : IFileUtilities
	{
		/// <summary>
		/// check if a file exists
		/// </summary>
		/// <param name="path">pathname to check</param>
		/// <returns>true if the file exists</returns>
        public bool FileExists(string path)
		{
			return File.Exists(path);
		}

        /// <summary>
        /// rename / move a file
        /// </summary>
        /// <param name="sourceFileName">source pathname</param>
        /// <param name="destinationFileName">destination pathname</param>
        public void FileRename(string sourceFileName, string destinationFileName)
        {
            FileRename(sourceFileName,destinationFileName,false);
        }

        /// <summary>
        /// rename / move a file
        /// </summary>
        /// <param name="sourceFileName">source pathname</param>
        /// <param name="destinationFileName">destination pathname</param>
        /// <param name="allowOverwrite">set to true to overwrite an existing destination file</param>
        public void FileRename(string sourceFileName, string destinationFileName, bool allowOverwrite)
        {
            if (allowOverwrite)
            {
                if (File.Exists(destinationFileName))
                {
                    File.Delete(destinationFileName);
                }
            }
            File.Move(sourceFileName, destinationFileName);
        }

        /// <summary>
	    /// copy a file - will not overwrite an existing file
	    /// the containing folder will be created if it does not exist
	    /// </summary>
	    /// <param name="sourceFileName">source pathname</param>
	    /// <param name="destinationFileName">destination pathname</param>
	    public void FileCopy(string sourceFileName, string destinationFileName)
		{
			FileCopy(sourceFileName, destinationFileName, false);
		}

	    /// <summary>
	    /// copy a file - the containing folder will be created if it does not exist
	    /// </summary>
	    /// <param name="sourceFileName">source pathname</param>
	    /// <param name="destinationFileName">destination pathname</param>
	    /// <param name="allowOverwrite">set to true to overwrite an existing file</param>
	    public void FileCopy(string sourceFileName, string destinationFileName, bool allowOverwrite)
		{
			// Make sure directory exists
			var destinationDirectory = Path.GetDirectoryName(destinationFileName);
			if (destinationDirectory != null)
			{
				Directory.CreateDirectory(destinationDirectory);
			}

			File.Copy(sourceFileName, destinationFileName, allowOverwrite);
		}

	    /// <summary>
	    /// delete a file
	    /// </summary>
	    /// <param name="path">pathname of the file to delete</param>
	    public void FileDelete(string path)
		{
			File.Delete(path);
		}
	}
}