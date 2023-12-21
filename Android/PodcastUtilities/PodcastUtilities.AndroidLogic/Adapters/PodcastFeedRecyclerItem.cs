using PodcastUtilities.Common.Configuration;

namespace PodcastUtilities.AndroidLogic.Adapters
{
    public class PodcastFeedRecyclerItem
    {
        public string Id { get; set; }
        public IPodcastInfo PodcastFeed { get; set; }
    }
}