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
            public EventHandler<Tuple<string, Html.IImageGetter>> SetText;
        }
        public ObservableGroup Observables = new ObservableGroup();

        private Application ApplicationContext;
        private ILogger Logger;
        private IFileSystemHelper FileSystemHelper;
        private Html.IImageGetter ImageGetter;

        public HelpViewModel(Application app, ILogger logger, IFileSystemHelper fileSystemHelper, Html.IImageGetter imageGetter) : base(app)
        {
            ApplicationContext = app;
            Logger = logger;
            Logger.Debug(() => $"HelpViewModel:ctor");
            FileSystemHelper = fileSystemHelper;
            ImageGetter = imageGetter;
        }
        public void Initialise()
        {
            Logger.Debug(() => $"HelpViewModel:Initialise");
            Observables.SetText?.Invoke(this,Tuple.Create(GetHelpText(), ImageGetter));
        }

        private string GetHelpText()
        {
            return FileSystemHelper.GetAssetsFileContents("help/help.html", false);
        }
    }
}