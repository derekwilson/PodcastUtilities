using PodcastUtilities.Common.Feeds;

namespace PodcastUtilities.AndroidLogic.ViewModel.Share
{
    public class ShareEpisodeRecyclerItem
    {
        public required string Id { get; set; }
        public required IPodcastFeedItem Episode { get; set; }
    }
}
