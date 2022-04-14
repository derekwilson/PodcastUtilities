using Android.App;
using AndroidX.Lifecycle;
using PodcastUtilities.AndroidLogic.Logging;
using PodcastUtilities.AndroidLogic.Utilities;
using System;

namespace PodcastUtilities.AndroidLogic.ViewModel.Settings
{
    public class OpenSourceLicensesViewModel : AndroidViewModel, ILifecycleObserver
    {
        public class ObservableGroup
        {
            public EventHandler<Tuple<string, string>> AddText;
            public EventHandler ScrollToTop;
        }
        public ObservableGroup Observables = new ObservableGroup();

        private Application ApplicationContext;
        private ILogger Logger;
        private IResourceProvider ResourceProvider;
        private IFileSystemHelper FileSystemHelper;

        public OpenSourceLicensesViewModel(
            Application app,
            ILogger logger,
            IResourceProvider resProvider,
            IFileSystemHelper fsHelper
            ) : base(app)
        {
            Logger = logger;
            Logger.Debug(() => $"OpenSourceLicensesViewModel:ctor");

            ApplicationContext = app;
            ResourceProvider = resProvider;
            FileSystemHelper = fsHelper;
        }

        public void Initialise()
        {
            Logger.Debug(() => $"OpenSourceLicensesViewModel:Initialise");
            AddAllLicenseFiles();
        }

        private void AddAllLicenseFiles()
        {
            // to add new licenses to this page just place the new licenses text file in the license folder in the assets
            // each file should just be text, the first line is treated as the title the rest as the body text
            // subfolder are supported, files and subfolders are sorted alphabetically
            // to remove a license, delete the file from the license folder in the assets

            var files = FileSystemHelper.GetAssetsFolderFiles("license");
            if (files == null)
            {
                return;
            }
            foreach (var file in files)
            {
                Logger.Debug(() => $"OpenSourceLicensesViewModel:AddAllLicenseFiles found file {file}");
                AddLicenseText(file);
            };
            Observables.ScrollToTop?.Invoke(this, null);
        }

        private void AddLicenseText(string file)
        {
            var text = FileSystemHelper.GetAssetsFileContents(file, true);
            var lineSplit = text.IndexOf("\n");
            if (lineSplit != -1)
            {
                Observables.AddText?.Invoke(this, Tuple.Create(text.Substring(0, lineSplit), text.Substring(lineSplit + 1)));
            }
            else
            {
                Observables.AddText?.Invoke(this, Tuple.Create("", text));
            }
        }
    }
}