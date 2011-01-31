using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace PodcastUtilities.Common
{
	public class ControlFile : XmlDocument
	{
        private const string DefaultSortField = "Name";
        private const string DefaultSortDirection = "ascending";
        
        private string _filename;

		public ControlFile(string filename)
		{
			_filename = filename;

			Load(_filename);
		}

        private string GetNodeText(string xpath)
		{
			XmlNode n = SelectSingleNode(xpath);
			if (n == null)
			{
				throw new System.Exception("GetNodeText : Node path '" + xpath + "' not found");
			}
			return n.InnerText;
		}

		public string GetNodeText(XmlNode root, string xpath)
		{
			XmlNode n = root.SelectSingleNode(xpath);
			if (n == null)
			{
				throw new System.Exception("GetNodeText : Node path '" + xpath + "' not found");
			}
			return n.InnerText;
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
                }
                return 0;
            }
        }

        public string SortField
        {
            get
            {
                try
                {
                    return GetNodeText("podcasts/global/sortfield");
                }
                catch
                {
                    return DefaultSortField;
                }
            }
        }

        public string SortDirection
        {
            get
            {
                try
                {
                    return GetNodeText("podcasts/global/sortdirection");
                }
                catch
                {
                    return DefaultSortDirection;
                }
            }
        }
    }
}
