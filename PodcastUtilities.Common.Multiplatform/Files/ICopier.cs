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
using System.Collections.Generic;

namespace PodcastUtilities.Common.Files
{
    /// <summary>
    /// supports the ability to copy a number of SyncItem
    /// </summary>
    public interface ICopier : IStatusUpdate
	{
        /// <summary>
        /// perform the copy operation
        /// </summary>
        /// <param name="sourceFiles">the list of SyncItem s to be copied</param>
        /// <param name="sourceRootPath">the root pathname of the source</param>
        /// <param name="destinationRootPath">the root pathname to the destination</param>
        /// <param name="freeSpaceToLeaveOnDestination">free space to meave on the destination in MB</param>
        /// <param name="whatif">true to emit all the status updates but not actually perform the copy, false to do the copy</param>
        void CopyFilesToTarget(IEnumerable<FileSyncItem> sourceFiles, string sourceRootPath, string destinationRootPath, long freeSpaceToLeaveOnDestination, bool whatif);
	}
}