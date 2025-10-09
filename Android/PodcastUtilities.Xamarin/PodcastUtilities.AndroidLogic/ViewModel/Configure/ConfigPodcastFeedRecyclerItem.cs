using PodcastUtilities.Common.Configuration;

namespace PodcastUtilities.AndroidLogic.ViewModel.Configure
{
    public class ConfigPodcastFeedRecyclerItem
    {
        public string Id { get; set; }
        public IPodcastInfo PodcastFeed { get; set; }
    }

}