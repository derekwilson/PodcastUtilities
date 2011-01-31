using System.IO;
using System.Linq;

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

		public bool Exists
		{
			get { return _directoryInfo.Exists; }
		}

		public string FullName
		{
			get { return _directoryInfo.FullName; }
		}

		public IFileInfo[] GetFiles(string pattern)
		{
			var realFiles = _directoryInfo.GetFiles(pattern);

			return realFiles.Select(f => new SystemFileInfo(f)).ToArray();
		}

		public void Create()
		{
			_directoryInfo.Create();
		}
	}
}