using PodcastUtilities.Common.Configuration;

namespace PodcastUtilities.AndroidLogic.ViewModel.Main
{
    public class PodcastFeedRecyclerItem
    {
        public required string Id { get; set; }
        public required IPodcastInfo PodcastFeed { get; set; }
    }
}