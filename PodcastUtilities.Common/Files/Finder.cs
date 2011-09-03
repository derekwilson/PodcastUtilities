using System.Collections.Generic;
using System.IO;
using System.Linq;
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


        private IEnumerable<IFileInfo> GetSortedFiles(IDirectoryInfo src, string pattern, string sortField, bool ascendingSort)
        {
            try
            {
                var fileList = new List<IFileInfo>(src.GetFiles(pattern));

                FileSorter.Sort(fileList, sortField, ascendingSort);

                return fileList;
            }
            catch (DirectoryNotFoundException)
            {
                // if the folder is not there then there is nothing to do
                return new List<IFileInfo>();
            }
        }

    }
}
