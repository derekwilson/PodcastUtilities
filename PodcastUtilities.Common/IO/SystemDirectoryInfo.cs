using System.IO;

namespace PodcastUtilities.Common.IO
{
	internal class SystemDirectoryInfo : IDirectoryInfo
	{
		private readonly DirectoryInfo _directoryInfo;

		public SystemDirectoryInfo(string path)
			: this(new DirectoryInfo(path))
		{
		}

		public SystemDirectoryInfo(DirectoryInfo directoryInfo)
		{
			_directoryInfo = directoryInfo;
		}

		public IDirectoryInfo Root
		{
			get { return new SystemDirectoryInfo(_directoryInfo.Root); }
		}

		public string FullName
		{
			get { return _directoryInfo.FullName; }
		}
	}
}