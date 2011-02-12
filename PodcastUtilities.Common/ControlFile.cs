using System;
using System.Collections.Generic;
using System.Xml;

namespace PodcastUtilities.Common
{
	public class ControlFile : IControlFile
	{
        private const string DefaultSortField = "Name";
        private const string DefaultSortDirection = "ascending";
        
        private readonly XmlDocument _xmlDocument;

		private List<PodcastInfo> _podcasts;

		public ControlFile(string filename)
		{
			_xmlDocument = new XmlDocument();

			_xmlDocument.Load(filename);

			ReadPodcasts();
		}

        /// <summary>
        /// for testing
        /// </summary>
        public ControlFile(XmlDocument document)
		{
		    _xmlDocument = document;

            ReadPodcasts();
		}

		public string SourceRoot
		{
			get { return GetNodeText("podcasts/global/sourceRoot"); }
		}
		
		public string DestinationRoot
		{
			get { return GetNodeText("podcasts/global/destinationRoot"); }
		}

		public string PlaylistFilename
		{
			get { return GetNodeText("podcasts/global/playlistFilename"); }
		}

        public PlaylistFormat PlaylistFormat
        {
            get
            {
                string format = GetNodeText("podcasts/global/playlistFormat").ToLower();
                switch (format)
                {
                    case "wpl":
                        return PlaylistFormat.WPL;
                    case "asx":
                        return PlaylistFormat.ASX;
                }
                throw new IndexOutOfRangeException(string.Format("{0} is not a valid value for the playlist format",format));
            }
        }

        public long FreeSpaceToLeaveOnDestination
        {
            get
            {
                try
                {
                    return Convert.ToInt64(GetNodeText("podcasts/global/freeSpaceToLeaveOnDestinationMB"));
                }
                catch
                {
					return 0;
                }
            }
        }

		public IList<PodcastInfo> Podcasts
		{
			get { return _podcasts; }
		}

		public string SortField
        {
            get
            {
				return GetNodeTextOrDefault("podcasts/global/sortfield", DefaultSortField);
            }
        }

        public string SortDirection
        {
            get
            {
            	return GetNodeTextOrDefault("podcasts/global/sortdirection", DefaultSortDirection);
            }
        }


		private void ReadPodcasts()
		{
			_podcasts = new List<PodcastInfo>();

			var podcastNodes = _xmlDocument.SelectNodes("podcasts/podcast");

			if (podcastNodes != null)
			{
				foreach (XmlNode podcastNode in podcastNodes)
				{
					var podcastInfo = new PodcastInfo
					{
						Folder = GetNodeText(podcastNode, "folder"),
						Pattern = GetNodeText(podcastNode, "pattern"),
						MaximumNumberOfFiles = Convert.ToInt32(GetNodeTextOrDefault(podcastNode, "number", "-1")),
						SortField = GetNodeTextOrDefault(podcastNode, "sortfield", SortField),
						AscendingSort = !(GetNodeTextOrDefault(podcastNode, "sortdirection", SortDirection).ToLower().StartsWith("desc"))
					};

					_podcasts.Add(podcastInfo);
				}
			}
		}

		private string GetNodeText(string xpath)
		{
			return GetNodeText(_xmlDocument, xpath);
		}

		private string GetNodeTextOrDefault(string xpath, string defaultText)
		{
			return GetNodeTextOrDefault(_xmlDocument, xpath, defaultText);
		}

		private static string GetNodeText(XmlNode root, string xpath)
		{
			XmlNode n = root.SelectSingleNode(xpath);
			if (n == null)
			{
				throw new System.Exception("GetNodeText : Node path '" + xpath + "' not found");
			}
			return n.InnerText;
		}

		private static string GetNodeTextOrDefault(XmlNode root, string xpath, string defaultText)
		{
			XmlNode n = root.SelectSingleNode(xpath);

			return ((n != null) ? n.InnerText : defaultText);
		}
	}
}
