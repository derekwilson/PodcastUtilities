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
			FileCopy(sourceFileName, destinationFileName, false);
		}

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

		public void FileDelete(string path)
		{
			File.Delete(path);
		}
	}
}