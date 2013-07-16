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
    /// methods to query files in the physical file system and abstract away the file system from the main body of code
    /// </summary>
    class SystemFileInfo : IFileInfo
	{
		private readonly FileInfo _fileInfo;

        /// <summary>
        /// construct from anothe absract object
        /// </summary>
        /// <param name="fileInfo">object to construct from</param>
        public SystemFileInfo(FileInfo fileInfo)
		{
			_fileInfo = fileInfo;
		}

		#region Implementation of IFileInfo

        /// <summary>
        /// the name of the file eg. file.ext
        /// </summary>
        public string Name
		{
			get { return _fileInfo.Name; }
		}

        /// <summary>
        /// the full pathname of the object eg. c:\media\file.ext
        /// </summary>
        public string FullName
		{
			get { return _fileInfo.FullName; }
		}

        /// <summary>
        /// date time the file was created
        /// </summary>
        public DateTime CreationTime
		{
			get { return _fileInfo.CreationTime; }
		}

        ///<summary>
        /// Length of the file in bytes
        ///</summary>
        public long Length
        {
            get { return _fileInfo.Length; }
        }

        #endregion
	}
}