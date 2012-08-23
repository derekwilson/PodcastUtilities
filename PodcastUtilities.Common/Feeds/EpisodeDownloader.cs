using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using PodcastUtilities.Common.Exceptions;
using PodcastUtilities.Common.Perfmon;
using PodcastUtilities.Common.Platform;

namespace PodcastUtilities.Common.Feeds
{
    /// <summary>
    /// a task that will download a podcast episode on a background thread
    /// </summary>
    public class EpisodeDownloader : IEpisodeDownloader
    {
        private readonly IWebClientFactory _webClientFactory;
        private readonly IDirectoryInfoProvider _directoryInfoProvider;
        private readonly IFileUtilities _fileUtilities;
        private readonly IStateProvider _stateProvider;
        private readonly ICounterFactory _counterFactory;
        private readonly ICommandExecuter _commandExecuter;

        private readonly object _lock = new object();
        private IWebClient _client;
        private bool _started;
        private bool _complete;
        private ISyncItem _syncItem;
        private int _progressPercentage;
        private long _bytesDownloaded;
        private Stopwatch _stopWatch;

        /// <summary>
        /// the item to download
        /// </summary>
        public ISyncItem SyncItem
        {
            get
            {
                lock (_lock)
                {
                    return _syncItem;
                }
            }
            set
            {
                lock (_lock)
                {
                    _syncItem = value;
                }
            }
        }

        /// <summary>
        /// fire an event for an update in terms of progress or warning/error message
        /// </summary>
        public event EventHandler<StatusUpdateEventArgs> StatusUpdate;

        /// <summary>
        /// event for progress
        /// </summary>
        public event EventHandler<ProgressEventArgs> ProgressUpdate;

        /// <summary>
        /// the event that is fired when the task completes
        /// </summary>
        public EventWaitHandle TaskComplete { get; protected set; }

        /// <summary>
        /// gets the display name for the task
        /// </summary>
        public string Name
        {
            get
            {
                lock (_lock)
                {
                    if (SyncItem == null)
                    {
                        return "<UNSTARTED TASK>";
                    }
                    return SyncItem.EpisodeTitle;
                }
            }
        }

        /// <summary>
        /// create a task
        /// </summary>
        public EpisodeDownloader(IWebClientFactory webClientFactory, IDirectoryInfoProvider directoryInfoProvider, IFileUtilities fileUtilities, IStateProvider stateProvider, ICounterFactory counterFactory, ICommandExecuter commandExecuter)
        {
            _webClientFactory = webClientFactory;
            _commandExecuter = commandExecuter;
            _counterFactory = counterFactory;
            _stateProvider = stateProvider;
            _fileUtilities = fileUtilities;
            _directoryInfoProvider = directoryInfoProvider;
            TaskComplete = new ManualResetEvent(false);
        }

        /// <summary>
        /// start running the task - the task is started in the background and the method will return
        /// </summary>
        public void Start(object state)
        {
            lock (_lock)
            {
                if (SyncItem == null)
                {
                    throw new DownloaderException("SyncItem has not been set before calling the downloader");
                }

                if (_started || _client != null)
                {
                    throw new DownloaderException("Cannot start the task twice");
                }

                _started = true;
                _progressPercentage = 0;
                _bytesDownloaded = 0;
                _stopWatch = new Stopwatch();
                _stopWatch.Start();

                _client = _webClientFactory.CreateWebClient();
                _client.ProgressUpdate += new EventHandler<ProgressEventArgs>(ClientProgressUpdate);
                _client.DownloadFileCompleted += new AsyncCompletedEventHandler(ClientDownloadFileCompleted);

                CreateFolderIfNeeded();
                _client.DownloadFileAsync(SyncItem.EpisodeUrl, GetDownloadFilename(), SyncItem);
            }
        }

        private void CreateFolderIfNeeded()
        {
            string folder = Path.GetDirectoryName(_syncItem.DestinationPath);
            IDirectoryInfo dir = _directoryInfoProvider.GetDirectoryInfo(folder);
            if (!dir.Exists)
            {
                dir.Create();
            }
            if (_fileUtilities.FileExists(_syncItem.DestinationPath))
            {
                _fileUtilities.FileDelete(_syncItem.DestinationPath);
            }
        }

        private string GetDownloadFilename()
        {
            return Path.ChangeExtension(SyncItem.DestinationPath, "partial");
        }

        void ClientProgressUpdate(object sender, ProgressEventArgs e)
        {
            lock (_lock)
            {
                if (_progressPercentage == e.ProgressPercentage)
                {
                    // only report progress if we have moved on in percentage terms
                    return;
                }
                _progressPercentage = e.ProgressPercentage;
                _bytesDownloaded = e.ItemsProcessed;
            }

            OnProgressUpdate(e);
        }

        private void OnProgressUpdate(ProgressEventArgs e)
        {
            if (ProgressUpdate != null)
                ProgressUpdate(this, e);
        }

        private int ConvertBytesToMB(long bytes)
        {
            long kb = 0;
            double mb = 0;

            if (bytes > 0)
            {
                kb = (bytes / 1024);
            }
            if (kb > 0)
            {
                mb = (kb / 1024);
            }
            return (int)mb;
        }

        void ClientDownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            StatusUpdateEventArgs args = null;
            lock (_lock)
            {
                var syncItem = e.UserState as SyncItem;
                if (syncItem == null)
                {
                    args = new StatusUpdateEventArgs(StatusUpdateLevel.Error, "Missing token from download completed");
                }
                else if (e.Cancelled)
                {
                    args = new StatusUpdateEventArgs(StatusUpdateLevel.Status, string.Format(CultureInfo.InvariantCulture, "{0} Cancelled",syncItem.EpisodeTitle));
                }
                else if (e.Error != null && e.Error.InnerException != null) 
                {
                    args = new StatusUpdateEventArgs(StatusUpdateLevel.Error, string.Format(CultureInfo.InvariantCulture, "{0} {1}", syncItem.EpisodeTitle, e.Error.InnerException.Message), e.Error.InnerException);
                }
                else if (e.Error != null)
                {
                    args = new StatusUpdateEventArgs(StatusUpdateLevel.Error, string.Format(CultureInfo.InvariantCulture, "{0} {1}", syncItem.EpisodeTitle, e.Error.Message), e.Error);
                }
                else
                {
                    args = new StatusUpdateEventArgs(StatusUpdateLevel.Status, string.Format(CultureInfo.InvariantCulture, "{0} Completed", syncItem.EpisodeTitle));

                    _fileUtilities.FileRename(GetDownloadFilename(), _syncItem.DestinationPath, true);
                    RecordHighTideMark(syncItem);
                    ExecutePostDownloadCommand();
                }

                OnStatusUpdate(args);

                _client.Dispose();
                _client = null;

                _complete = true;
                _counterFactory.CreateAverageCounter(CategoryInstaller.PodcastUtilitiesCommonCounterCategory,
                                                     CategoryInstaller.AverageTimeToDownload,
                                                     CategoryInstaller.NumberOfDownloads).RegisterTime(_stopWatch);
                _counterFactory.CreateAverageCounter(CategoryInstaller.PodcastUtilitiesCommonCounterCategory,
                                                     CategoryInstaller.AverageMBDownload,
                                                     CategoryInstaller.SizeOfDownloads).RegisterValue(ConvertBytesToMB(_bytesDownloaded));
                TaskComplete.Set();
            }
        }

        private void ExecutePostDownloadCommand()
        {
            if (_syncItem.PostDownloadCommand != null)
            {
                OnStatusUpdate(new StatusUpdateEventArgs(StatusUpdateLevel.Status, string.Format(CultureInfo.InvariantCulture, "Executing: {0} {1}", _syncItem.PostDownloadCommand.Command,_syncItem.PostDownloadCommand.Arguments), null));
                try
                {
                    string output = _commandExecuter.ExecuteCommand(_syncItem.PostDownloadCommand.Command, _syncItem.PostDownloadCommand.Arguments, _syncItem.PostDownloadCommand.WorkingDirectory);
                    OnStatusUpdate(new StatusUpdateEventArgs(StatusUpdateLevel.Status, string.Format(CultureInfo.InvariantCulture, "{0}", output), null));
                }
                catch (Exception ex)
                {
                    OnStatusUpdate(new StatusUpdateEventArgs(StatusUpdateLevel.Error, string.Format(CultureInfo.InvariantCulture, "{0} {1}", _syncItem.EpisodeTitle, ex.Message), ex));
                }
            }
        }

        private void RecordHighTideMark(SyncItem syncItem)
        {
            var retry = 5;
            do
            {
                try
                {
                    var state = _stateProvider.GetState(_syncItem.StateKey);
                    state.DownloadHighTide = _syncItem.Published;
                    state.SaveState(_syncItem.StateKey);
                    retry = 0;
                }
                catch (System.IO.IOException)
                {
                    OnStatusUpdate(new StatusUpdateEventArgs(StatusUpdateLevel.Warning, string.Format(CultureInfo.InvariantCulture, "{0}, cannot write to state file, will retry", syncItem.EpisodeTitle), null));
                    if (_syncItem.RetryWaitTimeInSeconds > 0)
                    {
                        Thread.Sleep(1000 * _syncItem.RetryWaitTimeInSeconds);
                    }
                    retry--;
                    if (retry == 0)
                    {
                        throw;
                    }
                }
            } while (retry > 0);
        }

        private void OnStatusUpdate(StatusUpdateEventArgs e)
        {
            if (StatusUpdate != null)
                StatusUpdate(this, e);
        }

        /// <summary>
        /// cancel a background task - or prevent a task from running
        /// </summary>
        public void Cancel()
        {
            lock (_lock)
            {
                if (_client != null)
                {
                    _client.CancelAsync();
                }
                else
                {
                    // probably an unstarted task or a task that has completed
                    _complete = true;
                    TaskComplete.Set();
                }
            }
        }

        /// <summary>
        /// true if the task has been started
        /// </summary>
        public bool IsStarted()
        {
            lock (_lock)
            {
                return _started;
            }
        }

        /// <summary>
        /// true if the task has been completed - or canceled
        /// </summary>
        public bool IsComplete()
        {
            lock (_lock)
            {
                return _complete;
            }
        }
    }
}
