using Android.App;
using AndroidX.Lifecycle;
using PodcastUtilities.AndroidLogic.Logging;
using PodcastUtilities.AndroidLogic.Utilities;
using PodcastUtilities.Common.Configuration;
using System;
using static Android.Renderscripts.Sampler;

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
        }
        public ObservableGroup Observables = new ObservableGroup();

        private Application ApplicationContext;
        private ILogger Logger;
        private IResourceProvider ResourceProvider;
        private IApplicationControlFileProvider ApplicationControlFileProvider;
        private ICrashReporter CrashReporter;
        private IAnalyticsEngine AnalyticsEngine;
        private IClipboardHelper ClipboardHelper;

        private bool ShouldCheckClipboard = true;

        public AddFeedViewModel(
            Application app,
            ILogger logger,
            IResourceProvider resProvider,
            IApplicationControlFileProvider appControlFileProvider,
            ICrashReporter crashReporter,
            IAnalyticsEngine analyticsEngine,
            IClipboardHelper clipboardHelper) : base(app)
        {
            Logger = logger;
            Logger.Debug(() => $"AddFeedViewModel:ctor");

            ApplicationContext = app;
            ResourceProvider = resProvider;
            ApplicationControlFileProvider = appControlFileProvider;
            CrashReporter = crashReporter;
            AnalyticsEngine = analyticsEngine;
            ClipboardHelper = clipboardHelper;
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

        public void TestFeed(string text)
        {
            Logger.Debug(() => $"AddFeedViewModel:TestFeed {text}");
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
    }
}