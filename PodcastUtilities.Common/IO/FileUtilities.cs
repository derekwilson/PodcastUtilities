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
			// Make sure directory exists
			var destinationDirectory = Path.GetDirectoryName(destinationFileName);
			if (destinationDirectory != null)
			{
				Directory.CreateDirectory(destinationDirectory);
			}

			File.Copy(sourceFileName, destinationFileName);
		}

		public void FileDelete(string path)
		{
			File.Delete(path);
		}
	}
}