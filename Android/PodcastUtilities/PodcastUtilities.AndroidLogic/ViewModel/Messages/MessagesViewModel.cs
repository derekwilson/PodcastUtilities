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
        }
        public ObservableGroup Observables = new ObservableGroup();

        private Application ApplicationContext;
        private ILogger Logger;
        private IResourceProvider ResourceProvider;
        private IStatusAndProgressMessageStore Store;

        public MessagesViewModel(
            Application app,
            ILogger logger,
            IResourceProvider resProvider,
            IStatusAndProgressMessageStore store
            ) : base(app)
        {
            Logger = logger;
            Logger.Debug(() => $"MessagesViewModel:ctor");

            ApplicationContext = app;
            ResourceProvider = resProvider;
            Store = store;
        }

        public void Initialise()
        {
            Logger.Debug(() => $"MessagesViewModel:Initialise");
            Observables.AddText?.Invoke(this, Store.GetAllMessages());
            Observables.ScrollToTop?.Invoke(this, null);
        }

    }
}