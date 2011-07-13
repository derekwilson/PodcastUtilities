using System.Collections.Generic;
using System.Linq;
using PodcastUtilities.Common.Platform;

namespace PodcastUtilities.Common
{
    /// <summary>
    /// finds files
    /// </summary>
    public class FileFinder
		: IFileFinder
    {
    	/// <summary>
    	/// construct the copier
    	/// </summary>
    	/// <param name="fileSorter">object used to sort the files</param>
    	/// <param name="directoryInfoProvider">astract access to the file system</param>
        public FileFinder(IFileSorter fileSorter, IDirectoryInfoProvider directoryInfoProvider)
    	{
    		FileSorter = fileSorter;
    		DirectoryInfoProvider = directoryInfoProvider;
    	}


    	private IFileSorter FileSorter { get; set; }
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
        public List<IFileInfo> GetFiles(
			string folderPath,
			string pattern,
			int maximumNumberOfFiles,
			string sortField,
			bool ascendingSort)
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
        public List<IFileInfo> GetFiles(
			string folderPath,
			string pattern)
		{
			var directoryInfo = DirectoryInfoProvider.GetDirectoryInfo(folderPath);

			return directoryInfo.GetFiles(pattern).ToList();
		}


        private IEnumerable<IFileInfo> GetSortedFiles(IDirectoryInfo src, string pattern, string sortField, bool ascendingSort)
        {
            var fileList = new List<IFileInfo>(src.GetFiles(pattern));

        	FileSorter.Sort(fileList, sortField, ascendingSort);

            return fileList;
        }

    }
}
