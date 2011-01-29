namespace PodcastUtilities.Common.IO
{
	public interface IDirectoryInfo
	{
		IDirectoryInfo Root { get; }

		string FullName { get; }

		bool Exists { get; }

		IFileInfo[] GetFiles(string pattern);

		void Create();
	}
}