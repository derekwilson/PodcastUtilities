using Android.Content;
using AndroidX.Lifecycle;
using PodcastUtilities.AndroidLogic.Logging;
using PodcastUtilities.AndroidLogic.Utilities;
using PodcastUtilities.AndroidLogic.ViewModel.Purge;
using PodcastUtilities.Common;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Feeds;

namespace PodcastUtilities.AndroidLogic.ViewModel.Share
{
    public class ShareEpisodeViewModel : AndroidViewModel, ILifecycleObserver
    {
        public class ObservableGroup
        {
            public EventHandler<string>? Title;
            public EventHandler<string>? DisplayMessage;
            public EventHandler? StartProgress;
            public EventHandler? EndProgress;
            public EventHandler<List<ShareEpisodeRecyclerItem>>? SetItems;
            public EventHandler<Tuple<string, Intent>>? DisplayChooser;
        }
        public ObservableGroup Observables = new ObservableGroup();

        private ILogger Logger;
        private IResourceProvider ResourceProvider;
        private IApplicationControlFileProvider ApplicationControlFileProvider;
        private ICrashReporter CrashReporter;
        private IAnalyticsEngine AnalyticsEngine;
        private IPodcastFeedFactory FeedFactory;
        private IWebClientFactory WebClientFactory;
        private IShareProvider ShareProvider;

        // do not make this anything other than private
        private object SyncLock = new object();

        private int PodcastFeedToEditId = -1;
        private IPodcastInfo? FeedToDownload = null;

        private List<ShareEpisodeRecyclerItem> AllItems = new List<ShareEpisodeRecyclerItem>(20);
        private bool StartedFindingItems = false;
        private bool CompletedFindingItems = false;

        public ShareEpisodeViewModel(
            Application app,
            ILogger logger,
            IResourceProvider resourceProvider,
            IApplicationControlFileProvider applicationControlFileProvider,
            ICrashReporter crashReporter,
            IAnalyticsEngine analyticsEngine,
            IPodcastFeedFactory feedFactory,
            IWebClientFactory webClientFactory,
            IShareProvider shareProvider) : base(app)
        {
            Logger = logger;
            ResourceProvider = resourceProvider;
            ApplicationControlFileProvider = applicationControlFileProvider;
            CrashReporter = crashReporter;
            AnalyticsEngine = analyticsEngine;
            FeedFactory = feedFactory;
            WebClientFactory = webClientFactory;
            ShareProvider = shareProvider;
        }

        public void Initialise(string id)
        {
            Logger.Debug(() => $"ShareEpisodeViewModel:Initialise - {id}");
            PodcastFeedToEditId = Convert.ToInt32(id);
        }

        private IPodcastInfo? GetFeedToDownloadEpisodesFrom()
        {
            var controlFile = ApplicationControlFileProvider.GetApplicationConfiguration();
            int index = 0;
            foreach (var podcastInfo in controlFile.GetPodcasts())
            {
                if (index == PodcastFeedToEditId)
                {
                    if (podcastInfo.Feed == null || podcastInfo.Feed.Address == null)
                    {
                        Logger.Debug(() => $"ShareEpisodeViewModel:GetFeedToDownloadEpisodesFrom podcast has no feed {PodcastFeedToEditId}");
                        return null;
                    }
                    else
                    {
                        return podcastInfo;
                    }
                }
                index++;
            }
            Logger.Debug(() => $"EditFeedViewModel:GetFeedToEdit cannot find podcast {PodcastFeedToEditId}");
            return null;
        }

        public void FindItemsInFeed()
        {
            Logger.Debug(() => $"ShareEpisodeViewModel:FindItemsInFeed");
            FeedToDownload = GetFeedToDownloadEpisodesFrom();
            if (FeedToDownload == null)
            {
                Logger.Warning(() => $"ShareEpisodeViewModel:FindItemsInFeed bad feed {PodcastFeedToEditId}");
                CrashReporter.LogNonFatalException(new InvalidOperationException($"Bad feed: {PodcastFeedToEditId}"));
                return;
            }

            lock (SyncLock)
            {
                if (StartedFindingItems)
                {
                    Logger.Warning(() => $"ShareEpisodeViewModel:FindItemsInFeed - ignoring, already initialised");
                    if (CompletedFindingItems)
                    {
                        Observables.SetItems?.Invoke(this, AllItems);
                    }
                    else
                    {
                        Observables.StartProgress?.Invoke(this, EventArgs.Empty);
                    }
                    return;
                }
                StartedFindingItems = true;
            }

            Observables.StartProgress?.Invoke(this, EventArgs.Empty);

            // lets find some items
            AllItems.Clear();
            int index = 0;
            try
            {
                using (var webClient = WebClientFactory.CreateWebClient())
                {
                    var downloader = new Downloader(webClient, FeedFactory);
                    var feed = downloader.DownloadFeed(FeedToDownload.Feed.Format.Value, FeedToDownload.Feed.Address, null);
                    // we want to consider the episodes from latest to earliest
                    // maybe we want to restrict the number of items displayed?
                    var episodes = feed.Episodes.OrderByDescending(item => item.Published);

                    foreach (IPodcastFeedItem podcastFeedItem in episodes)
                    {
                        var item = new ShareEpisodeRecyclerItem() 
                        {
                            Id = index.ToString(),      // we use the position in the list as the id
                            Episode = podcastFeedItem 
                        };
                        AllItems.Add(item);
                        index++;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(() => $"ShareEpisodeViewModel:FindItemsInFeed", ex);
                CrashReporter.LogNonFatalException(ex);
            }

            CompletedFindingItems = true;
            Observables.EndProgress?.Invoke(this, EventArgs.Empty);
            Observables.SetItems?.Invoke(this, AllItems);
            AnalyticsEngine.ShareEpisodeScanEvent(AllItems.Count);
        }

        internal string? GetEpisodeItemLabel(IPodcastFeedItem episode)
        {
            return episode.EpisodeTitle;
        }

        internal string? GetEpisodeItemSubLabel(IPodcastFeedItem episode)
        {
            return episode.Published.ToShortDateString();
        }

        internal void EpisodeItemSelected(IPodcastFeedItem episode)
        {
            Logger.Debug(() => $"ShareEpisodeViewModel:EpisodeItemSelected: {episode.EpisodeTitle}");

            var intent = ShareProvider.GetEpisodeSharingIntent(FeedToDownload, episode);
            Observables.DisplayChooser?.Invoke(this, Tuple.Create(ResourceProvider.GetString(Resource.String.share_episode_chooser_title), intent));
            AnalyticsEngine.ShareEpisodeEvent($"{FeedToDownload?.Folder}:{episode.EpisodeTitle}");
        }
    }
}
