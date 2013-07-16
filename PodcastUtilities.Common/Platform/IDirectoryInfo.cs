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
namespace PodcastUtilities.Common.Platform
{
    /// <summary>
    /// methods to interact with directories in the physical file system and abstract away the file system from the main body of code
    /// </summary>
    public interface IDirectoryInfo
	{
		/// <summary>
		/// the full pathname of the directory
		/// </summary>
        string FullName { get; }

		/// <summary>
		/// true if it exists
		/// </summary>
        bool Exists { get; }

		/// <summary>
		/// gets an abstract collection of files that are contained by by the directory
		/// </summary>
		/// <param name="pattern">a search patter for example *.mp3</param>
		/// <returns>a collection of abstracted files</returns>
        IFileInfo[] GetFiles(string pattern);

        /// <summary>
        /// gets an abstract collection of directories that are contained by the directory
        /// </summary>
        /// <param name="pattern">a search patter for example *.*</param>
        /// <returns>a collection of abstracted files</returns>
        IDirectoryInfo[] GetDirectories(string pattern);

        /// <summary>
        /// create the directory in the file system
        /// </summary>
        void Create();

        /// <summary>
        /// delete the directory in the file system
        /// </summary>
        void Delete();
    }
}