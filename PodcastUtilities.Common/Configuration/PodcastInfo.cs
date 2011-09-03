namespace PodcastUtilities.Common.Configuration
{
	/// <summary>
	/// an individual podcast
	/// </summary>
    public class PodcastInfo
	{
	    ///<summary>
	    /// Podcast ctor
	    ///</summary>
	    public PodcastInfo()
	    {
	        Feed = new FeedInfo();
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
		public string SortField { get; set; }
		/// <summary>
		/// true for an ascending sort, false for a descending
		/// </summary>
        public bool AscendingSort { get; set; }
		/// <summary>
		/// maximum number of files to copy, -1 for unlimited
		/// </summary>
        public int MaximumNumberOfFiles { get; set; }
        /// <summary>
        /// the configuration info for the feed
        /// </summary>
        public FeedInfo Feed { get; set; }
	}
}