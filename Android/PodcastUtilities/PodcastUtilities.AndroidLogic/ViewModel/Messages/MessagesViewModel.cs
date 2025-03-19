using Android.App;
using AndroidX.Lifecycle;
using PodcastUtilities.AndroidLogic.Logging;
using PodcastUtilities.AndroidLogic.MessageStore;
using PodcastUtilities.AndroidLogic.Utilities;
using System;
using System.Threading.Tasks;

namespace PodcastUtilities.AndroidLogic.ViewModel.Messages
{
    public class MessagesViewModel : AndroidViewModel, ILifecycleObserver
    {
        public class ObservableGroup
        {
            public EventHandler<string> AddText;
            public EventHandler ScrollToTop;
            public EventHandler ResetText;
            public EventHandler StartLoading;
            public EventHandler EndLoading;
        }
        public ObservableGroup Observables = new ObservableGroup();

        private Application ApplicationContext;
        private ILogger Logger;
        private IResourceProvider ResourceProvider;
        private IStatusAndProgressMessageStore Store;
        private IAnalyticsEngine AnalyticsEngine;

        public MessagesViewModel(
            Application app,
            ILogger logger,
            IResourceProvider resProvider,
            IStatusAndProgressMessageStore store,
            IAnalyticsEngine analyticsEngine) : base(app)
        {
            Logger = logger;
            Logger.Debug(() => $"MessagesViewModel:ctor");

            ApplicationContext = app;
            ResourceProvider = resProvider;
            Store = store;
            AnalyticsEngine = analyticsEngine;
        }

        public void Initialise()
        {
            Logger.Debug(() => $"MessagesViewModel:Initialise");
            LoadMessagesInBackground();
        }

        // public for testing
        public Task LoadMessagesInBackground()
        {
            return Task.Run(() => LoadMessages());
        }

        // dont do this on the UI thread
        private void LoadMessages()
        {
            try
            {
                Observables.StartLoading?.Invoke(this, null);
                AnalyticsEngine.ViewPageEvent(IAnalyticsEngine.Page_Logs, Store.GetTotalNumberOfLines());
                Observables.ResetText?.Invoke(this, null);
                Observables.AddText?.Invoke(this, Store.GetAllMessages());
            }
            finally
            {
                Observables.EndLoading?.Invoke(this, null);
                DelayedScrollToTop();       // if you dont delay - it doesnt work
            }
        }

        public Task DelayedScrollToTop()
        {
            return Task.Delay(1000).ContinueWith((_) => Observables.ScrollToTop(this, null));
        }
    }
}