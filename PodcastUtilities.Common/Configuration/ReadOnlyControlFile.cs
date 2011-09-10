using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;
using PodcastUtilities.Common.Exceptions;
using PodcastUtilities.Common.Playlists;

namespace PodcastUtilities.Common.Configuration
{
    /// <summary>
	/// this object represents the xml control file
	/// </summary>
    public class ReadOnlyControlFile : BaseControlFile, IReadOnlyControlFile
	{
		/// <summary>
		/// create the object and read the control file from the specified filename
		/// </summary>
		/// <param name="fileName">pathname to the control file xml</param>
        public ReadOnlyControlFile(string fileName)
		{
            LoadFromFile(fileName);
		}

        /// <summary>
        /// only used for unit testing
        /// </summary>
        public ReadOnlyControlFile(IXPathNavigable document)
		{
            LoadFromXml(document);
		}
	}
}
