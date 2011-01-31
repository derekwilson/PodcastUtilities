using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PodcastUtilities.Common
{
	public class SyncItem
	{
		public FileInfo Source { get; set; }
		public string DestinationPath { get; set; }
		public bool Copied { get; set; }
	}
}
