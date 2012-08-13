using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace PodcastUtilities.Common.Configuration
{
	/// <summary>
	/// an individual podcast
	/// </summary>
    public class PodcastInfo : IPodcastInfo
	{
        private readonly IControlFileGlobalDefaults _controlFileGlobalDefaults;

        ///<summary>
	    /// Podcast ctor
	    ///</summary>
	    public PodcastInfo(IControlFileGlobalDefaults controlFileGlobalDefaults)
        {
            _controlFileGlobalDefaults = controlFileGlobalDefaults;
            Pattern = new DefaultableReferenceTypeItem<string>(_controlFileGlobalDefaults.GetDefaultFilePattern);
            AscendingSort = new DefaultableValueTypeItem<bool>(_controlFileGlobalDefaults.GetDefaultAscendingSort);
            SortField = new DefaultableValueTypeItem<PodcastFileSortField>(_controlFileGlobalDefaults.GetDefaultSortField);
            MaximumNumberOfFiles = new DefaultableValueTypeItem<int>(_controlFileGlobalDefaults.GetDefaultNumberOfFiles);
        }

	    /// <summary>
		/// the folder relative to the source root that contains the media for the podcast
		/// </summary>
        public string Folder { get; set; }
		/// <summary>
		/// file pattern for the media files eg. *.mp3
		/// </summary>
        public IDefaultableItem<string> Pattern { get; set; }
        /// <summary>
        /// field to sort on "creationtime" to use the file created time anything else to use the file name
        /// </summary>
        public IDefaultableItem<PodcastFileSortField> SortField { get; set; }
		/// <summary>
		/// true for an ascending sort, false for a descending
		/// </summary>
        public IDefaultableItem<bool> AscendingSort { get; set; }
		/// <summary>
		/// maximum number of files to copy, -1 for unlimited
		/// </summary>
        public IDefaultableItem<int> MaximumNumberOfFiles { get; set; }
        /// <summary>
        /// the configuration info for the feed
        /// </summary>
        public IFeedInfo Feed { get; set; }
        /// <summary>
        /// command to be executed after the podcast has been downloaded
        /// </summary>
        public ITokenisedCommand PostDownloadCommand { get; set; }

        /// <summary>
        /// create a feed in the podcast
        /// </summary>
        public void CreateFeed()
        {
            Feed = new FeedInfo(_controlFileGlobalDefaults);
        }

        /// <summary>
        /// create a post download command in the podcast
        /// </summary>
        public void CreatePostDownloadCommand()
        {
            PostDownloadCommand = new TokenisedCommand(_controlFileGlobalDefaults);
        }

        /// <summary>
	    /// Creates a new object that is a copy of the current instance.
	    /// </summary>
	    /// <returns>
	    /// A new object that is a copy of this instance.
	    /// </returns>
	    /// <filterpriority>2</filterpriority>
	    public object Clone()
	    {
            var copy = new PodcastInfo(_controlFileGlobalDefaults);
            XmlSerializationHelper.CloneUsingXmlSerialization("podcast", this, copy);
            return copy;
	    }

	    /// <summary>
	    /// This method is reserved and should not be used. When implementing the IXmlSerializable interface, you should return null (Nothing in Visual Basic) from this method, and instead, if specifying a custom schema is required, apply the <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute"/> to the class.
	    /// </summary>
	    /// <returns>
	    /// An <see cref="T:System.Xml.Schema.XmlSchema"/> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)"/> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)"/> method.
	    /// </returns>
	    public XmlSchema GetSchema()
	    {
	        return null;
	    }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader"/> stream from which the object is deserialized. 
        ///                 </param>
        public void ReadXml(XmlReader reader)
        {
            XmlSerializationHelper.ProcessElement(reader, "podcast", ProcessPodcastElements);
        }

        private ProcessorResult ProcessPodcastElements(XmlReader reader)
	    {
            var result = ProcessorResult.Processed;

            var elementName = reader.LocalName;

            if (elementName == "feed")
            {
                Feed = new FeedInfo(_controlFileGlobalDefaults);
                Feed.ReadXml(reader);
                return result;
            }
            if (elementName == "postdownloadcommand")
            {
                PostDownloadCommand = new TokenisedCommand(_controlFileGlobalDefaults);
                PostDownloadCommand.ReadXml(reader);
                return result;
            }

            if (!reader.IsEmptyElement)
            {
                reader.Read();
            }
            var content = reader.Value.Trim();

            switch (elementName)
            {
                case "folder":
                    Folder = content;
                    break;
                case "pattern":
                    Pattern.Value = content;
                    break;
                case "number":
                    MaximumNumberOfFiles.Value = Convert.ToInt32(content, CultureInfo.InvariantCulture);
                    break;
                case "sortfield":
                    SortField.Value = ReadSortField(content);
                    break;
                case "sortdirection":
                    AscendingSort.Value = ReadSortDirection(content);
                    break;
                default:
                    result = ProcessorResult.Ignored;
                    break;
            }
            return result;
        }

	    /// <summary>
	    /// Converts an object into its XML representation.
	    /// </summary>
	    /// <param name="writer">The <see cref="T:System.Xml.XmlWriter"/> stream to which the object is serialized. 
	    ///                 </param>
	    public void WriteXml(XmlWriter writer)
	    {
            writer.WriteElementString("folder", Folder);
            if (Pattern.IsSet)
            {
                writer.WriteElementString("pattern", Pattern.Value);
            }
            if (MaximumNumberOfFiles.IsSet)
            {
                writer.WriteElementString("number", MaximumNumberOfFiles.Value.ToString(CultureInfo.InvariantCulture));
            }
            if (SortField.IsSet)
            {
                writer.WriteElementString("sortfield", WriteSortField(SortField.Value));
            }
            if (AscendingSort.IsSet)
            {
                writer.WriteElementString("sortdirection", WriteSortDirection(AscendingSort.Value));
            }
            if (Feed != null)
            {
                writer.WriteStartElement("feed");
                Feed.WriteXml(writer);
                writer.WriteEndElement();
            }
            if (PostDownloadCommand != null)
            {
                writer.WriteStartElement("postdownloadcommand");
                PostDownloadCommand.WriteXml(writer);
                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// parse the sort field
        /// </summary>
        /// <param name="sortField"></param>
        /// <returns></returns>
        public static PodcastFileSortField ReadSortField(string sortField)
        {
            switch (sortField.ToUpperInvariant())
            {
                case "CREATIONTIME":
                    return PodcastFileSortField.CreationTime;
                default:
                    return PodcastFileSortField.FileName;
            }
        }

        /// <summary>
        /// convert the sortfield for serialisation
        /// </summary>
        /// <returns></returns>
        public static string WriteSortField(PodcastFileSortField sortField)
	    {
	        switch (sortField)
	        {
	            case PodcastFileSortField.CreationTime:
                    return "creationtime";
                default:
                    return "name";
            }
	    }

        /// <summary>
        /// parse the sort direction
        /// </summary>
        /// <param name="sortDirection"></param>
        /// <returns></returns>
        public static bool ReadSortDirection(string sortDirection)
        {
            return !sortDirection.ToUpperInvariant().StartsWith("DESC", StringComparison.Ordinal);
        }

        /// <summary>
        /// convert the sortdirection for serialisation
        /// </summary>
        /// <returns></returns>
        public static string WriteSortDirection(bool ascendingSort)
	    {
	        if (ascendingSort)
	        {
	            return "asc";
	        }
	        return "desc";
	    }
	}
}