using PodcastUtilities.Common.Configuration;

namespace PodcastUtilities.AndroidLogic.ViewModel.Configure
{
    public class ConfigPodcastFeedRecyclerItem
    {
        public required string Id { get; set; }
        public required IPodcastInfo PodcastFeed { get; set; }
    }

}