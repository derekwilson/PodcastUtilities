using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
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
        /// create an empty control file
        /// </summary>
        public ReadOnlyControlFile() : base()
        {
        }

		/// <summary>
		/// create the object and read the control file from the specified filename
		/// </summary>
		/// <param name="fileName">pathname to the control file xml</param>
        public ReadOnlyControlFile(string fileName)
		{
            XmlReaderSettings readSettings = new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Fragment };

            FileStream fileStream = new FileStream(fileName, FileMode.Open);

            ReadXml(XmlReader.Create(fileStream, readSettings));
        }

        /// <summary>
        /// only used for unit testing
        /// </summary>
        public ReadOnlyControlFile(XmlReader xml)
        {
            ReadXml(xml);
        }

        /// <summary>
        /// only used for unit testing
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode")]
        public ReadOnlyControlFile(XmlDocument document)
        {
            MemoryStream stream = new MemoryStream();
            document.Save(stream);
            stream.Position = 0;
            ReadXml(XmlReader.Create(stream));
        }

        /// <summary>
        /// only used for unit testing
        /// </summary>
        public ReadOnlyControlFile(Stream stream)
        {
            ReadXml(XmlReader.Create(stream));
        }
    }
}
