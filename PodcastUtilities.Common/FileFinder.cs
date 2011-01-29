using System.Collections.Generic;
using System.Linq;
using PodcastUtilities.Common.IO;

namespace PodcastUtilities.Common
{
    public class FileFinder
		: IFileFinder
    {
    	public FileFinder(IFileSorter fileSorter, IDirectoryInfoProvider directoryInfoProvider)
    	{
    		FileSorter = fileSorter;
    		DirectoryInfoProvider = directoryInfoProvider;
    	}

    	public FileFinder()
			: this(new FileSorter(), new SystemDirectoryInfoProvider())
    	{
    	}


    	private IFileSorter FileSorter { get; set; }
    	private IDirectoryInfoProvider DirectoryInfoProvider { get; set; }


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
