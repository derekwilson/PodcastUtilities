using Android.App;
using AndroidX.Lifecycle;
using PodcastUtilities.AndroidLogic.Logging;
using PodcastUtilities.AndroidLogic.Utilities;
using System;

namespace PodcastUtilities.AndroidLogic.ViewModel.Messages
{
    public class MessagesViewModel : AndroidViewModel, ILifecycleObserver
    {
        public class ObservableGroup
        {
            public EventHandler<string> AddText;
            public EventHandler ScrollToTop;
            public EventHandler ResetText;
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
            AnalyticsEngine.ViewLogsEvent(Store.GetTotalNumberOfLines());
            Observables.ResetText?.Invoke(this, null);
            Observables.AddText?.Invoke(this, Store.GetAllMessages());
            Observables.ScrollToTop?.Invoke(this, null);
        }

    }
}