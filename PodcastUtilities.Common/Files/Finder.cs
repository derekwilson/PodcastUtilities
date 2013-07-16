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
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Platform;

namespace PodcastUtilities.Common.Files
{
    /// <summary>
    /// finds files
    /// </summary>
    public class Finder
		: IFinder
    {
    	/// <summary>
    	/// construct the copier
    	/// </summary>
    	/// <param name="fileSorter">object used to sort the files</param>
    	/// <param name="directoryInfoProvider">astract access to the file system</param>
        public Finder(ISorter fileSorter, IDirectoryInfoProvider directoryInfoProvider)
    	{
    		FileSorter = fileSorter;
    		DirectoryInfoProvider = directoryInfoProvider;
    	}


    	private ISorter FileSorter { get; set; }
    	private IDirectoryInfoProvider DirectoryInfoProvider { get; set; }


        /// <summary>
        /// gets a number of files that match a given pattern
        /// </summary>
        /// <param name="folderPath">folder to look in</param>
        /// <param name="pattern">pattern to look for eg. *.mp3</param>
        /// <param name="maximumNumberOfFiles">maximum number of files to find</param>
        /// <param name="sortField">field to sort on</param>
        /// <param name="ascendingSort">true to sort ascending false to sort descending</param>
        /// <returns></returns>
        public IList<IFileInfo> GetFiles(string folderPath, string pattern, int maximumNumberOfFiles, PodcastFileSortField sortField, bool ascendingSort)
		{
			var directoryInfo = DirectoryInfoProvider.GetDirectoryInfo(folderPath);

			var sortedFiles = GetSortedFiles(directoryInfo, pattern, sortField, ascendingSort);
			if (maximumNumberOfFiles >= 0)
			{
				sortedFiles = sortedFiles.Take(maximumNumberOfFiles);
			}

			return sortedFiles.ToList();
		}

        /// <summary>
        /// gets all the files that match a given pattern
        /// </summary>
        /// <param name="folderPath">folder to look in</param>
        /// <param name="pattern">pattern to look for eg. *.mp3</param>
        /// <returns></returns>
        public IList<IFileInfo> GetFiles(
			string folderPath,
			string pattern)
		{
            try
            {
                var directoryInfo = DirectoryInfoProvider.GetDirectoryInfo(folderPath);

                return directoryInfo.GetFiles(pattern).ToList();
            }
            catch (DirectoryNotFoundException)
            {
                // if the folder is not there then there is nothing to do
                return new List<IFileInfo>();
            }
        }


        private IEnumerable<IFileInfo> GetSortedFiles(IDirectoryInfo src, string pattern, PodcastFileSortField sortField, bool ascendingSort)
        {
            try
            {
                var fileList = new Collection<IFileInfo>(src.GetFiles(pattern));

                return FileSorter.Sort(fileList, sortField, ascendingSort);
            }
            catch (DirectoryNotFoundException)
            {
                // if the folder is not there then there is nothing to do
                return new Collection<IFileInfo>();
            }
        }

    }
}
