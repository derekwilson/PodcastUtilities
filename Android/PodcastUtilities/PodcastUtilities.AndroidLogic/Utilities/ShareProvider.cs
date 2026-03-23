using Android.Content;
using PodcastUtilities.AndroidLogic.Logging;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Feeds;

namespace PodcastUtilities.AndroidLogic.Utilities
{
    public interface IShareProvider
    {
        Intent GetFeedSharingIntent(IPodcastInfo podcastFeed);
        Intent GetEpisodeSharingIntent(IPodcastInfo? feedToDownload, IPodcastFeedItem episode);
    }

    public class ShareProvider : IShareProvider
    {
        private ILogger Logger;
        private IResourceProvider ResourceProvider;

        public ShareProvider(ILogger logger, IResourceProvider resourceProvider)
        {
            Logger = logger;
            ResourceProvider = resourceProvider;
        }

        public Intent GetEpisodeSharingIntent(IPodcastInfo? feedToDownload, IPodcastFeedItem episode)
        {
            var subject = string.Format(ResourceProvider.GetString(Resource.String.share_episode_subject),
                episode.EpisodeTitle
                );
            var body = string.Format(ResourceProvider.GetString(Resource.String.share_episode_body),
                episode.EpisodeTitle,
                feedToDownload?.Folder,
                episode.Published.ToLongDateString(),
                episode.Address.ToString(),
                ResourceProvider.GetString(Resource.String.share_advert_app_url)
                );
            return GetSendSharingIntent(subject, body);
        }

        public Intent GetFeedSharingIntent(IPodcastInfo podcastFeed)
        {
            var subject = string.Format(ResourceProvider.GetString(Resource.String.share_feed_subject),
                podcastFeed.Folder
                );
            var body = string.Format(ResourceProvider.GetString(Resource.String.share_feed_body),
                podcastFeed.Folder,
                podcastFeed.Feed.Address,
                ResourceProvider.GetString(Resource.String.share_advert_app_url)
                );
            return GetSendSharingIntent(subject, body);
        }

        private Intent GetSendSharingIntent(string subject, string shareText)
        {
            Intent sharingIntent = new Intent(Intent.ActionSend);
            sharingIntent.SetType("text/plain");
            sharingIntent.PutExtra(Intent.ExtraSubject, subject);
            sharingIntent.PutExtra(Intent.ExtraText, shareText);
            return sharingIntent;
        }
    }
}
