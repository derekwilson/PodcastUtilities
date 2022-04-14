using Android.App;
using AndroidX.Lifecycle;
using PodcastUtilities.AndroidLogic.Logging;
using PodcastUtilities.AndroidLogic.Utilities;
using System;

namespace PodcastUtilities.AndroidLogic.ViewModel.Help
{
    public class HelpViewModel : AndroidViewModel, ILifecycleObserver
    {
        public class ObservableGroup
        {
            public EventHandler<string> SetText;
        }
        public ObservableGroup Observables = new ObservableGroup();

        private Application ApplicationContext;
        private ILogger Logger;
        private IResourceProvider ResourceProvider;
        private IFileSystemHelper FileSystemHelper;

        public HelpViewModel(Application app, ILogger logger, IResourceProvider resourceProvider, IFileSystemHelper fileSystemHelper) : base(app)
        {
            ApplicationContext = app;
            Logger = logger;
            Logger.Debug(() => $"HelpViewModel:ctor");
            ResourceProvider = resourceProvider;
            FileSystemHelper = fileSystemHelper;
        }
        public void Initialise()
        {
            Logger.Debug(() => $"HelpViewModel:Initialise");
            Observables.SetText?.Invoke(this, GetHelpText());
        }

        private string GetHelpText()
        {
            return FileSystemHelper.GetAssetsFileContents("help/help.html", false);
        }
    }
}