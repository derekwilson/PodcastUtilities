using Android.App;
using Android.Text;
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
            public EventHandler<Tuple<string, Html.IImageGetter>>? SetText;
        }
        public ObservableGroup Observables = new ObservableGroup();

        private Application ApplicationContext;
        private ILogger Logger;
        private IFileSystemHelper FileSystemHelper;
        private Html.IImageGetter ImageGetter;
        private IAnalyticsEngine AnalyticsEngine;

        public HelpViewModel(
            Application app, 
            ILogger logger, 
            IFileSystemHelper fileSystemHelper, 
            Html.IImageGetter imageGetter, 
            IAnalyticsEngine analyticsEngine
        ) : base(app)
        {
            ApplicationContext = app;
            Logger = logger;
            Logger.Debug(() => $"HelpViewModel:ctor");
            FileSystemHelper = fileSystemHelper;
            ImageGetter = imageGetter;
            AnalyticsEngine = analyticsEngine;
        }
        public void Initialise()
        {
            Logger.Debug(() => $"HelpViewModel:Initialise");
            AnalyticsEngine.ViewPageEvent(IAnalyticsEngine.Page_Help, 0);
            Observables.SetText?.Invoke(this,Tuple.Create(GetHelpText(), ImageGetter));
        }

        private string GetHelpText()
        {
            return FileSystemHelper.GetAssetsFileContents("help/help.html", false);
        }
    }
}