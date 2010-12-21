namespace PodcastUtilities.Common.IO
{
	public interface IFileUtilities
	{
		bool FileExists(string path);

		void FileCopy(string sourceFileName, string destinationFileName);
	}
}
