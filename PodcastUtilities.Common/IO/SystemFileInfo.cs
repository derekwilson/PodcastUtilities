using System;
using System.IO;

namespace PodcastUtilities.Common.IO
{
	class SystemFileInfo : IFileInfo
	{
		private readonly FileInfo _fileInfo;

		public SystemFileInfo(string path)
			: this(new FileInfo(path))
		{
		}

		public SystemFileInfo(FileInfo fileInfo)
		{
			_fileInfo = fileInfo;
		}

		#region Implementation of IFileInfo

		public string Name
		{
			get { return _fileInfo.Name; }
		}

		public string FullName
		{
			get { return _fileInfo.FullName; }
		}

		public DateTime CreationTime
		{
			get { return _fileInfo.CreationTime; }
		}

		#endregion
	}
}