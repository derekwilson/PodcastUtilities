using Android.App;
using AndroidX.Lifecycle;
using PodcastUtilities.AndroidLogic.Logging;
using PodcastUtilities.AndroidLogic.Utilities;
using PodcastUtilities.Common;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Feeds;
using System;
using System.Threading.Tasks;

namespace PodcastUtilities.AndroidLogic.ViewModel.Configure
{
    public class AddFeedViewModel : AndroidViewModel, ILifecycleObserver
    {
        public class ObservableGroup
        {
            public EventHandler<string> DisplayMessage;
            public EventHandler<string> Folder;
            public EventHandler<string> Url;
            public EventHandler Exit;
            public EventHandler<string> DisplayErrorMessage;
            public EventHandler HideErrorMessage;
            public EventHandler StartDownloading;
            public EventHandler EndDownloading;
        }
        public ObservableGroup Observables = new ObservableGroup();

        private Application ApplicationContext;
        private ILogger Logger;
        private IResourceProvider ResourceProvider;
        private IApplicationControlFileProvider ApplicationControlFileProvider;
        private ICrashReporter CrashReporter;
        private IAnalyticsEngine AnalyticsEngine;
        private IClipboardHelper ClipboardHelper;
        private IPodcastFeedFactory FeedFactory;
        private IWebClientFactory WebClientFactory;

        private bool ShouldCheckClipboard = true;
        private bool DownloadingInProgress = false;
        // do not make this anything other than private
        private object SyncLock = new object();

        public AddFeedViewModel(
            Application app,
            ILogger logger,
            IResourceProvider resProvider,
            IApplicationControlFileProvider appControlFileProvider,
            ICrashReporter crashReporter,
            IAnalyticsEngine analyticsEngine,
            IClipboardHelper clipboardHelper,
            IPodcastFeedFactory feedFactory,
            IWebClientFactory webClientFactory) : base(app)
        {
            Logger = logger;
            Logger.Debug(() => $"AddFeedViewModel:ctor");

            ApplicationContext = app;
            ResourceProvider = resProvider;
            ApplicationControlFileProvider = appControlFileProvider;
            CrashReporter = crashReporter;
            AnalyticsEngine = analyticsEngine;
            ClipboardHelper = clipboardHelper;
            FeedFactory = feedFactory;
            WebClientFactory = webClientFactory;
        }

        [Lifecycle.Event.OnCreate]
        [Java.Interop.Export]
        public void OnCreate()
        {
            Logger.Debug(() => $"AddFeedViewModel:OnCreate");
        }

        [Lifecycle.Event.OnDestroy]
        [Java.Interop.Export]
        public void OnDestroy()
        {
            Logger.Debug(() => $"AddFeedViewModel:OnDestroy");
        }

        [Lifecycle.Event.OnResume]
        [Java.Interop.Export]
        public void OnResume()
        {
            Logger.Debug(() => $"AddFeedViewModel:OnResume");
        }

        [Lifecycle.Event.OnStop]
        [Java.Interop.Export]
        public void OnStop()
        {
            Logger.Debug(() => $"AddFeedViewModel:OnStop");
            ShouldCheckClipboard = true;
        }

        public void Initialise()
        {
            Logger.Debug(() => $"AddFeedViewModel:Initialise");
        }

        public void AddFeed(string folder, string feedUrl)
        {
            Logger.Debug(() => $"AddFeedViewModel:AddFeed {folder}, {feedUrl}");

            if (string.IsNullOrWhiteSpace(feedUrl))
            {
                Observables.DisplayMessage?.Invoke(this, ResourceProvider.GetString(Resource.String.bad_url));
                return;
            }
            if (!Uri.IsWellFormedUriString(feedUrl, UriKind.Absolute))
            {
                Observables.DisplayMessage?.Invoke(this, ResourceProvider.GetString(Resource.String.bad_url));
                return;
            }
            if (string.IsNullOrWhiteSpace(folder))
            {
                Observables.DisplayMessage?.Invoke(this, ResourceProvider.GetString(Resource.String.bad_folder_empty));
                return;
            }
            var controlFile = ApplicationControlFileProvider.GetApplicationConfiguration();
            var newPodcast = new PodcastInfo(controlFile);
            newPodcast.Folder = folder;
            newPodcast.Feed = new FeedInfo(controlFile);
            newPodcast.Feed.Address = new Uri(feedUrl);
            if (ApplicationControlFileProvider.AddPodcastIfFoldernameUnique(newPodcast))
            {
                AnalyticsEngine.AddPodcastEvent(folder);
                AnalyticsEngine.AddPodcastFeedEvent(feedUrl);
                Observables.Exit?.Invoke(this, null);
            }
            else
            {
                Observables.DisplayMessage?.Invoke(this, ResourceProvider.GetString(Resource.String.bad_folder_duplicate));
            }
        }

        public void CheckClipboardForUrl(string text)
        {
            if (!ShouldCheckClipboard)
            {
                return;
            }
            ShouldCheckClipboard = false;   // this is a one shot
            Logger.Debug(() => $"AddFeedViewModel:CheckClipboardForUrl {text}");

            if (!string.IsNullOrWhiteSpace(text)) {
                // we already have text
                return;
            }
            var clipUrl = ClipboardHelper.GetUrlIfAvailable();
            if (string.IsNullOrWhiteSpace(clipUrl))
            {
                return;
            }
            Observables.Url?.Invoke(this, clipUrl);
        }

        public Task TestFeed(string folder, string feedUrl)
        {
            Logger.Debug(() => $"AddFeedViewModel:TestFeed {feedUrl}");

            if (string.IsNullOrWhiteSpace(feedUrl))
            {
                Observables.DisplayErrorMessage?.Invoke(this, ResourceProvider.GetString(Resource.String.bad_url));
                return null;
            }
            if (!Uri.IsWellFormedUriString(feedUrl, UriKind.Absolute))
            {
                Observables.DisplayErrorMessage?.Invoke(this, ResourceProvider.GetString(Resource.String.bad_url));
                return null;
            }

            return Task.Run(() =>
                {
                    GetFeedChannelData(feedUrl, string.IsNullOrWhiteSpace(folder));
                }
            );
        }

        // dont run this on the UI thread
        private void GetFeedChannelData(string address, bool replaceFolder)
        {
            Logger.Debug(() => $"AddFeedViewModel:GetFeedChannelData");
            lock (SyncLock)
            {
                if (DownloadingInProgress)
                {
                    Logger.Warning(() => $"AddFeedViewModel:DownloadAllPodcasts - already in progress - ignored");
                    return;
                }
                DownloadingInProgress = true;
            }

            try
            {
                Observables.HideErrorMessage?.Invoke(this, null);
                Observables.StartDownloading?.Invoke(this, null);
                var controlFile = ApplicationControlFileProvider.GetApplicationConfiguration();
                using (var webClient = WebClientFactory.CreateWebClient())
                {
                    var downloader = new Downloader(webClient, FeedFactory);
                    var feed = downloader.DownloadFeed(controlFile.GetDefaultFeedFormat(), new Uri(address), null);
                    if (feed != null)
                    {
                        if (replaceFolder)
                        {
                            Observables.Folder?.Invoke(this, feed.Title);
                        }
                        var message = string.Format(ResourceProvider.GetString(Resource.String.add_feed_test_feed_ok_fmt), feed.Title);
                        Observables.DisplayErrorMessage?.Invoke(this, message);
                    }
                    else
                    {
                        // you would have liked to think we got an exception
                        Observables.DisplayErrorMessage?.Invoke(this, ResourceProvider.GetString(Resource.String.add_feed_test_feed_error));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(() => $"AddFeedViewModel:GetFeedChannelData", ex);
                CrashReporter.LogNonFatalException($"Adding: {address}", ex);
                Observables.DisplayErrorMessage?.Invoke(this, ex.Message);
            }
            finally
            {
                Observables.EndDownloading?.Invoke(this, null);
                DownloadingInProgress = false;
            }
        }
    }
}