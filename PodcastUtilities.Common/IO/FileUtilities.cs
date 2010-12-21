using System.IO;

namespace PodcastUtilities.Common.IO
{
	public class FileUtilities : IFileUtilities
	{
		public bool FileExists(string path)
		{
			return File.Exists(path);
		}

		public void FileCopy(string sourceFileName, string destinationFileName)
		{
			File.Copy(sourceFileName, destinationFileName);
		}
	}
}