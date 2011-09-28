using System;
using System.Globalization;
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
            AscendingSort = new DefaultableItem<bool>(_controlFileGlobalDefaults.GetDefaultAscendingSort);
            SortField = new DefaultableItem<PodcastFileSortField>(_controlFileGlobalDefaults.GetDefaultSortField);
        }

	    /// <summary>
		/// the folder relative to the source root that contains the media for the podcast
		/// </summary>
        public string Folder { get; set; }
		/// <summary>
		/// file pattern for the media files eg. *.mp3
		/// </summary>
        public string Pattern { get; set; }
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
        public int MaximumNumberOfFiles { get; set; }
        /// <summary>
        /// the configuration info for the feed
        /// </summary>
        public IFeedInfo Feed { get; set; }

	    /// <summary>
	    /// create a feed in the podcast
	    /// </summary>
	    public void CreateFeed()
	    {
            Feed = new FeedInfo(_controlFileGlobalDefaults);
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
	        var newInfo = new PodcastInfo(_controlFileGlobalDefaults)
	                   {
	                       Folder = Folder,
                           Pattern = Pattern,
                           SortField = SortField,
                           MaximumNumberOfFiles = MaximumNumberOfFiles
	                   };
            if (Feed != null)
            {
                newInfo.Feed = Feed.Clone() as IFeedInfo;
            }

            if (AscendingSort.IsSet)
            {
                newInfo.AscendingSort.Value = AscendingSort.Value;
            }

            return newInfo;
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
	        throw new NotImplementedException();
	    }

	    /// <summary>
	    /// Converts an object into its XML representation.
	    /// </summary>
	    /// <param name="writer">The <see cref="T:System.Xml.XmlWriter"/> stream to which the object is serialized. 
	    ///                 </param>
	    public void WriteXml(XmlWriter writer)
	    {
            writer.WriteElementString("folder", Folder);
            writer.WriteElementString("pattern", Pattern);
            writer.WriteElementString("number", MaximumNumberOfFiles.ToString(CultureInfo.InvariantCulture));
            if (SortField.IsSet)
            {
                writer.WriteElementString("sortfield", WriteSortField(SortField));
            }
            if (AscendingSort.IsSet)
            {
                writer.WriteElementString("sortdirection", WriteSortDirection(AscendingSort));
            }
            if (Feed != null)
            {
                Feed.WriteXml(writer);
            }
        }

	    private static string WriteSortField(IDefaultableItem<PodcastFileSortField> sortField)
	    {
	        switch (sortField.Value)
	        {
	            case PodcastFileSortField.CreationTime:
                    return "creationtime";
                default:
                    return "name";
            }
	    }

	    private static string WriteSortDirection(IDefaultableItem<bool> ascendingSort)
	    {
	        if (ascendingSort.Value)
	        {
	            return "asc";
	        }
	        return "desc";
	    }
	}
}