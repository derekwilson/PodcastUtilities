namespace PodcastUtilities.Common.IO
{
	public class SystemDirectoryInfoProvider : IDirectoryInfoProvider
	{
		public IDirectoryInfo GetDirectoryInfo(string path)
		{
			return new SystemDirectoryInfo(path);
		}
	}
}