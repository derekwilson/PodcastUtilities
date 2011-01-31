namespace PodcastUtilities.Common
{
	public class PodcastInfo
	{
		public string Folder { get; set; }
		public string Pattern { get; set; }
		public string SortField { get; set; }
		public bool AscendingSort { get; set; }
		public int MaximumNumberOfFiles { get; set; }
	}
}