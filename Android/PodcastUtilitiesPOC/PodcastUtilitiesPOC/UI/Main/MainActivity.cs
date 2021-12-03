using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.DocumentFile.Provider;
using PodcastUtilities.Common;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Feeds;
using PodcastUtilities.Common.Platform;
using PodcastUtilitiesPOC.CustomViews;
using PodcastUtilitiesPOC.UI.Download;
using PodcastUtilitiesPOC.UI.Example;
using PodcastUtilitiesPOC.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PodcastUtilitiesPOC.UI.Main
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private const int REQUEST_SELECT_FILE = 3000;
        private const int REQUEST_SELECT_FOLDER = 3001;

        private AndroidApplication AndroidApplication;
        private IPreferencesProvider PreferencesProvider;
        private IDriveInfoProvider DriveInfoProvider;

        private string OverrideRoot;
        private string ControlFileUri;
        private int NoOfFeeds = 0;
        List<ISyncItem> AllEpisodes = new List<ISyncItem>(20);
        private ITaskPool TaskPool;
        private int number_of_files_to_download = 0;
        private int number_of_files_downloaded = 0;
        private bool reported_driveinfo_error = false;

        private readonly StringBuilder OutputBuffer = new StringBuilder(1000);
        static object SyncLock = new object();

        private ProgressSpinnerView ProgressSpinner;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            AndroidApplication = Application as AndroidApplication;
            AndroidApplication.Logger.Debug(() => "MainActivity:OnCreate");
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            DriveInfoProvider = AndroidApplication.IocContainer.Resolve<IDriveInfoProvider>();
            PreferencesProvider = AndroidApplication.IocContainer.Resolve<IPreferencesProvider>();
            ControlFileUri = PreferencesProvider.GetPreferenceString(ApplicationContext.GetString(Resource.String.prefs_control_uri_key), "");
            AndroidApplication.Logger.Debug(() => $"MainActivity:OnCreate Conrol Uri = {ControlFileUri}");

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            // Get our UI controls from the loaded layout
            List<string> envirnment = WindowsEnvironmentInformationProvider.GetEnvironmentRuntimeDisplayInformation();
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(AndroidApplication.DisplayVersion);
            foreach (string line in envirnment)
            {
                builder.AppendLine(line);
            }
            SetTextViewText(Resource.Id.txtVersions, builder.ToString());

            builder.Clear();

            builder.AppendLine($"Personal Folder = {System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal)}");
            builder.AppendLine($"AppData Folder = {System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData)}");
            builder.AppendLine($"CommonAppData Folder = {System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData)}");
            Java.IO.File[] files = ApplicationContext.GetExternalFilesDirs(null);
            foreach (Java.IO.File file in files)
            {
                builder.AppendLine($"ExternalFile = {file.AbsolutePath}");
                OverrideRoot = file.AbsolutePath;
            }
            builder.AppendLine($"Podcasts Folder = {Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPodcasts).AbsolutePath}");
            SetTextViewText(Resource.Id.txtAppStorage, builder.ToString());

            ProgressSpinner = FindViewById<ProgressSpinnerView>(Resource.Id.progressBar);

            Button btnLoad = FindViewById<Button>(Resource.Id.btnLoadConfig);
            btnLoad.Click += (sender, e) => LoadConfig();

            Button btnFind = FindViewById<Button>(Resource.Id.btnFindPodcasts);
            // works
            btnFind.Click += (sender, e) => Task.Run(() => FindEpisodesToDownload());
            // crash - java.lang.RuntimeException: Can't create handler inside thread that has not called Looper.prepare()
            //btnFind.Click += async (sender, e) => await Task.Run(() => FindEpisodesToDownload());
            // blocks UI thread
            //btnFind.Click += async (sender, e) => FindEpisodesToDownload();

            Button btnSetRoot = FindViewById<Button>(Resource.Id.btnSetRoot);
            btnSetRoot.Click += (sender, e) => SetRoot();

            Button btnDownload = FindViewById<Button>(Resource.Id.btnDownload);
            btnDownload.Click += (sender, e) => Task.Run(() => Download());

            bool isReadonly = Android.OS.Environment.MediaMountedReadOnly.Equals(Android.OS.Environment.ExternalStorageState);
            bool isWriteable = Android.OS.Environment.MediaMounted.Equals(Android.OS.Environment.ExternalStorageState);

            if (!string.IsNullOrEmpty(ControlFileUri))
            {
                try
                {
                    // TODO - we need to ask permission if the file has been edited by another app
                    Android.Net.Uri uri = Android.Net.Uri.Parse(ControlFileUri);
                    AndroidApplication.ControlFile = OpenControlFile(uri);
                }
                catch (Exception ex)
                {
                    AndroidApplication.Logger.LogException(() => $"MainActivity: OnCreate", ex);
                    SetTextViewText(Resource.Id.txtConfigFilePath, $"Error {ex.Message}");
                }
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            //change main_compat_menu
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return base.OnCreateOptionsMenu(menu);
        }


        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            AndroidApplication.Logger.Debug(() => $"MainActivity:OnOptionsItemSelected {item.ItemId}");
            switch (item.ItemId)
            {
                case Resource.Id.action_download_podcasts:
                    if (AndroidApplication.ControlFile == null)
                    {
                        AndroidApplication.Logger.Warning(() => $"MainActivity:OnOptionsItemSelected - no control file");
                        ToastMessage("No control file selected");
                        return base.OnOptionsItemSelected(item);
                    }
                    var intent = new Intent(this, typeof(DownloadActivity));
                    StartActivity(intent);
                    break;
            case Resource.Id.action_example:
                StartActivity(new Intent(this, typeof(ExampleActivity)));
                break;
            }
            return base.OnOptionsItemSelected(item);
        }

        private void LoadConfig()
        {
            if (PermissionChecker.HasManageStoragePermission(this))
            {
                SelectFile();
            }
            else
            {
                AndroidApplication.Logger.Debug(() => $"MainActivity:LoadConfig - permission not granted - requesting");
                PermissionRequester.RequestManageStoragePermission(this, PermissionRequester.REQUEST_CODE_WRITE_EXTERNAL_STORAGE_PERMISSION, AndroidApplication.PackageName);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            AndroidApplication.Logger.Debug(() => $"MainActivity:OnRequestPermissionsResult code {requestCode}, res {grantResults.Length}");
            switch (requestCode)
            {
                // for manage storage on SDK30+ it will go to activity result - thanks google
                // also we get CANCELLED as the result code so its difficult to know if it worked
                case PermissionRequester.REQUEST_CODE_WRITE_EXTERNAL_STORAGE_PERMISSION:
                    if (grantResults.Length == 1 && grantResults[0] == Permission.Granted)
                    {
                        SelectFile();
                    }
                    else
                    {
                        ToastMessage("Permission Denied");
                    }
                    break;
                default:
                    Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
                    base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
                    break;
            }
        }

        private void SetRoot()
        {
            if (AndroidApplication.ControlFile == null)
            {
                AndroidApplication.Logger.Warning(() => $"MainActivity:SetRoot - no control file");
                AddLineToOutput("No control file");
                DisplayOutput();
                return;
            }
            SelectFolder();
        }

        private void SelectFile()
        {
            AndroidApplication.Logger.Debug(() => $"MainActivity:SelectFile");
            // ACTION_OPEN_DOCUMENT is the intent to choose a file via the system's file browser.
            var intent = new Intent(Intent.ActionOpenDocument);

            // Filter to only show results that can be "opened", such as a
            // file (as opposed to a list of contacts or timezones)
            intent.AddCategory(Intent.CategoryOpenable);

            // Filter using the MIME type.
            // If one wanted to search for ogg vorbis files, the type would be "audio/ogg".
            // To search for all documents available via installed storage providers, it would be "*/*".
            // as we know that other apps do not always report GPX MIME type correctly lets try for everything
            intent.SetType("*/*");

            StartActivityForResult(intent, REQUEST_SELECT_FILE);
        }

        public void SelectFolder()
        {
            AndroidApplication.Logger.Debug(() => $"MainActivity:SelectFolder");
            Intent intent = new Intent(Intent.ActionOpenDocumentTree);
            StartActivityForResult(intent, REQUEST_SELECT_FOLDER);
        }

        private ReadOnlyControlFile OpenControlFile(Android.Net.Uri uri)
        {
            try
            {
                ContentResolver resolver = Application.ContentResolver;
                var stream = resolver.OpenInputStream(uri);
                var xml = new XmlDocument();
                xml.Load(stream);
                var control = new ReadOnlyControlFile(xml);
                var podcasts = control.GetPodcasts();
                int count = 0;
                foreach (var item in podcasts)
                {
                    count++;
                }
                NoOfFeeds = count;

                AndroidApplication.Logger.Debug(() => $"MainActivity:Control Podcasts {control.GetSourceRoot()}");
                AndroidApplication.Logger.Debug(() => $"MainActivity:Control Podcasts {count}");

                SetTextViewText(Resource.Id.txtConfigFilePath, $"{uri.ToString()}");
                SetTextViewText(Resource.Id.txtOutput, $"{count}, {control.GetSourceRoot()}");
                return control;
            }
            catch (Exception ex)
            {
                AndroidApplication.Logger.LogException(() => $"MainActivity: OpenConfigFile", ex);
                SetTextViewText(Resource.Id.txtConfigFilePath, $"Error {ex.Message}");
                return null;
            }
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            AndroidApplication.Logger.Debug(() => $"MainActivity:OnActivityResult {requestCode}, {resultCode}");
            base.OnActivityResult(requestCode, resultCode, data);
            if (!resultCode.Equals(Result.Ok))
            {
                return;
            }

            AndroidApplication.Logger.Debug(() => $"MainActivity:OnActivityResult {data.Data.ToString()}");
            switch (requestCode)
            {
                // we asked for manage storage access in SDK30+
                case PermissionRequester.REQUEST_CODE_WRITE_EXTERNAL_STORAGE_PERMISSION:
                    SelectFile();
                    break;
                case REQUEST_SELECT_FILE:
                    ToastMessage("OK");
                    AndroidApplication.ControlFile = OpenControlFile(data.Data);
                    if (AndroidApplication.ControlFile != null)
                    {
                        PreferencesProvider.SetPreferenceString(ApplicationContext.GetString(Resource.String.prefs_control_uri_key), data.Data.ToString());
                        ApplicationContext.ContentResolver.TakePersistableUriPermission(data.Data, ActivityFlags.GrantReadUriPermission);
                    }
                    break;
                case REQUEST_SELECT_FOLDER:
                    ToastMessage("OK");
                    SetTextViewText(Resource.Id.txtRoot, $"{data.Data.ToString()}");

                    //Android.Net.Uri uri = data.Data;
                    //Android.Net.Uri docUri = DocumentsContract.BuildDocumentUriUsingTree(uri,DocumentsContract.GetTreeDocumentId(uri));
                    // TODO - write GetRealPathFromUri()
                    // see https://stackoverflow.com/questions/29713587/how-to-get-the-real-path-with-action-open-document-tree-intent
                    //String path = GetRealPathFromURI(uri);

                    DocumentFile file = DocumentFile.FromTreeUri(ApplicationContext, data.Data);
                    AndroidApplication.Logger.Debug(() => $"MainActivity:OnActivityResult {file.Uri.Path}");
                    SetTextViewText(Resource.Id.txtRoot, $"{file.Uri.Path}");
                    break;
            }
        }

        private string GetRealPathFromURI(Android.Net.Uri contentURI)
        {
            string result;
            var cursor = Application.ContentResolver.Query(contentURI, null, null, null, null);
            if (cursor == null)
            { // Source is Dropbox or other similar local file path
                result = contentURI.Path;
            }
            else
            {
                cursor.MoveToFirst();
                int idx = cursor.GetColumnIndex(MediaStore.Images.ImageColumns.Data);
                result = cursor.GetString(idx);
                cursor.Close();
            }
            return result;
        }

        private void FindEpisodesToDownload()
        {
            AndroidApplication.Logger.Debug(() => $"MainActivity:FindEpisodesToDownload");
            OutputBuffer.Clear();
            AddLineToOutput("Started");
            DisplayOutput();
            if (AndroidApplication.ControlFile == null)
            {
                AndroidApplication.Logger.Warning(() => $"MainActivity:FindEpisodesToDownload - no control file");
                AddLineToOutput("No control file");
                DisplayOutput();
                return;
            }

            StartProgress();
            ToastMessage("Started");
            IEpisodeFinder podcastEpisodeFinder = null;
            podcastEpisodeFinder = AndroidApplication.IocContainer.Resolve<IEpisodeFinder>();

            // find the episodes to download
            AllEpisodes.Clear();
            int count = 0;
            foreach (var podcastInfo in AndroidApplication.ControlFile.GetPodcasts())
            {
                var episodesInThisFeed = podcastEpisodeFinder.FindEpisodesToDownload(
                    // works on all OS's with just WRITE_EXTERNAL
                    //"/sdcard/Android/data/com.andrewandderek.podcastutilitiespoc.debug/files/PodcastEpisodes",
                    // works on OS4 with just WRITE_EXTERNAL, hangs on OS10 and fails with exception on OS11
                    // OS10 hangs unless <application android:requestLegacyExternalStorage="true" is in the manifest
                    //"/sdcard/PodcastUtilities/PodcastEpisodes",
                    // ApplicationContext.GetExternalFilesDirs(null); last folder in the array
                    //"/storage/82E7-140A/Android/data/com.andrewandderek.podcastutilitiespoc.debug/files"
                    //OverrideRoot,
                    AndroidApplication.ControlFile.GetSourceRoot(),
                    AndroidApplication.ControlFile.GetRetryWaitInSeconds(),
                    podcastInfo,
                    AndroidApplication.ControlFile.GetDiagnosticRetainTemporaryFiles());
                AllEpisodes.AddRange(episodesInThisFeed);
                foreach (var episode in episodesInThisFeed)
                {
                    AndroidApplication.Logger.Debug(() => $"MainActivity:FindEpisodesToDownload {episode.Id}, {episode.EpisodeTitle}");
                    AddLineToOutput(episode.EpisodeTitle);
                }
                count++;
                UpdateProgress(count);
            }
            AddLineToOutput("Done.");
            ToastMessage("Done");
            EndProgress();
            DisplayOutput();
        }

        private void Download()
        {
            AndroidApplication.Logger.Debug(() => $"MainActivity:Download");
            OutputBuffer.Clear();
            AddLineToOutput("Started");
            DisplayOutput();
            if (AndroidApplication.ControlFile == null || AllEpisodes.Count < 1)
            {
                AndroidApplication.Logger.Warning(() => $"MainActivity:Download - no control file or nothing to download");
                AddLineToOutput("No control file or nothing to download");
                DisplayOutput();
                return;
            }

            //StartProgress();
            ToastMessage("Started");

            number_of_files_to_download = AllEpisodes.Count;
            number_of_files_downloaded = 0;
            if (number_of_files_to_download > 0)
            {
                // convert them to tasks
                var converter = AndroidApplication.IocContainer.Resolve<ISyncItemToEpisodeDownloaderTaskConverter>();
                IEpisodeDownloader[] downloadTasks = converter.ConvertItemsToTasks(AllEpisodes, StatusUpdate, ProgressUpdate);

                foreach (var task in downloadTasks)
                {
                    AndroidApplication.Logger.Warning(() => $"MainActivity:Download to: {task.SyncItem.DestinationPath}");
                }

                // run them in a task pool
                TaskPool = AndroidApplication.IocContainer.Resolve<ITaskPool>();
                TaskPool.RunAllTasks(AndroidApplication.ControlFile.GetMaximumNumberOfConcurrentDownloads(), downloadTasks);
            }

            AddLineToOutput("Done.");
            ToastMessage("Done");
            //EndProgress();
            DisplayOutput();
        }

        void ProgressUpdate(object sender, ProgressEventArgs e)
        {
            lock (SyncLock)
            {
                // keep all the message together

                ISyncItem syncItem = e.UserState as ISyncItem;
                if (e.ProgressPercentage % 10 == 0)
                {
                    var line = string.Format("{0} ({1} of {2}) {3}%", syncItem.EpisodeTitle,
                                                    DisplayFormatter.RenderFileSize(e.ItemsProcessed),
                                                    DisplayFormatter.RenderFileSize(e.TotalItemsToProcess),
                                                    e.ProgressPercentage);
                    AddLineToOutput(line);
                    AndroidApplication.Logger.Debug(() => line);
                }

                if (e.ProgressPercentage == 100)
                {
                    number_of_files_downloaded++;
                    AddLineToOutput($"Completed {number_of_files_downloaded} of {number_of_files_to_download} downloads");
                }

                // TODO - fix IsDestinationDriveFull
                /*
                if (IsDestinationDriveFull(ControlFile.GetSourceRoot(), ControlFile.GetFreeSpaceToLeaveOnDownload()))
                {
                    if (TaskPool != null)
                    {
                        TaskPool.CancelAllTasks();
                    }
                }
                */
                DisplayOutput();
            }
        }

        bool IsDestinationDriveFull(string destinationRootPath, long freeSpaceToLeaveOnDestination)
        {
            long availableFreeSpace = 0;
            try
            {
                var driveInfo = DriveInfoProvider.GetDriveInfoForPath(Path.GetPathRoot(Path.GetFullPath(destinationRootPath)));
                availableFreeSpace = driveInfo.AvailableFreeSpace;
            }
            catch (Exception ex)
            {
                if (!reported_driveinfo_error)
                {
                    AddLineToOutput($"Cannot find available free space on drive, will continue to download. Error: {ex.Message}");
                }
                reported_driveinfo_error = true;
                return false;
            }

            long freeKb = 0;
            double freeMb = 0;
            if (availableFreeSpace > 0)
                freeKb = availableFreeSpace / 1024;
            if (freeKb > 0)
                freeMb = freeKb / 1024;

            AndroidApplication.Logger.Debug(() => $"MainActivity:IsDestinationDriveFull {freeMb}");
            if (freeMb < freeSpaceToLeaveOnDestination)
            {
                AddLineToOutput(string.Format("Destination drive is full leaving {0:#,0.##} MB free", freeMb));
                return true;
            }
            return false;
        }

        void StatusUpdate(object sender, StatusUpdateEventArgs e)
        {
            bool _verbose = false;
            if (e.MessageLevel == StatusUpdateLevel.Verbose && !_verbose)
            {
                return;
            }

            lock (SyncLock)
            {
                // keep all the message together
                if (e.Exception != null)
                {
                    AndroidApplication.Logger.LogException(() => $"MainActivity:StatusUpdate -> ", e.Exception);
                    AddLineToOutput(e.Message);
                    AddLineToOutput(string.Concat(" ", e.Exception.ToString()));
                }
                else
                {
                    AndroidApplication.Logger.Debug(() => $"MainActivity:StatusUpdate {e.Message}");
                    AddLineToOutput(e.Message);
                }
                DisplayOutput();
            }
        }

        private void AddLineToOutput(string line)
        {
            OutputBuffer.AppendLine(line);
        }

        private void DisplayOutput()
        {
            RunOnUiThread(() =>
            {
                try
                {
                    SetTextViewText(Resource.Id.txtOutput, $"{OutputBuffer.ToString()}");
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    AndroidApplication.Logger.LogException(() => $"MainActivity:AddLineToOutput - ignoring render error", ex);
                }
            });
        }

        private void StartProgress()
        {
            RunOnUiThread(() =>
            {
                ProgressViewHelper.StartProgress(ProgressSpinner, Window, NoOfFeeds);
            });
        }

        private void UpdateProgress(int position)
        {
            RunOnUiThread(() =>
            {
                ProgressSpinner.Progress = position;
            });
        }

        private void EndProgress()
        {
            RunOnUiThread(() =>
            {
                ProgressViewHelper.CompleteProgress(ProgressSpinner, Window);
            });
        }

        private void ToastMessage(string message)
        {
            RunOnUiThread(() =>
            {
                Toast.MakeText(Application.Context, message, ToastLength.Short).Show();
            });
        }

        private void SetTextViewText(int id, string txt)
        {
            TextView txtView = FindViewById<TextView>(id);
            txtView.Text = txt;
        }
    }
}