using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading;
using PodcastUtilities.Common.Exceptions;
using PodcastUtilities.Common.Platform;

namespace PodcastUtilities.Common
{
    /// <summary>
    /// a task that will download a podcast episode on a background thread
    /// </summary>
    public class PodcastEpisodeDownloader : IPodcastEpisodeDownloader
    {
        private readonly IWebClientFactory _webClientFactory;
        private readonly IDirectoryInfoProvider _directoryInfoProvider;
        private readonly IFileUtilities _fileUtilities;
        private readonly IStateProvider _stateProvider;

        private object _lock = new object();
        private IWebClient _client;
        private bool _started;
        private bool _complete;
        private IFeedSyncItem _syncItem;
        private int _progressPercentage;

        /// <summary>
        /// the item to download
        /// </summary>
        public IFeedSyncItem SyncItem
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
        /// create a task
        /// </summary>
        public PodcastEpisodeDownloader(IWebClientFactory webClientFactory, IDirectoryInfoProvider directoryInfoProvider, IFileUtilities fileUtilities, IStateProvider stateProvider)
        {
            _webClientFactory = webClientFactory;
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

                _client = _webClientFactory.GetWebClient();
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
            }

            OnProgressUpdate(e);
        }

        private void OnProgressUpdate(ProgressEventArgs e)
        {
            if (ProgressUpdate != null)
                ProgressUpdate(this, e);
        }

        void ClientDownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            StatusUpdateEventArgs args = null;
            lock (_lock)
            {
                var syncItem = e.UserState as FeedSyncItem;
                if (syncItem == null)
                {
                    args = new StatusUpdateEventArgs(StatusUpdateEventArgs.Level.Error, "Missing token from download completed");
                }
                else if (e.Cancelled)
                {
                    args = new StatusUpdateEventArgs(StatusUpdateEventArgs.Level.Status, string.Format("{0} Cancelled",syncItem.EpisodeTitle));
                }
                else if (e.Error != null && e.Error.InnerException != null) 
                {
                    args = new StatusUpdateEventArgs(StatusUpdateEventArgs.Level.Error, string.Format("{0} {1}", syncItem.EpisodeTitle, e.Error.InnerException.Message), e.Error.InnerException);
                }
                else if (e.Error != null)
                {
                    args = new StatusUpdateEventArgs(StatusUpdateEventArgs.Level.Error, string.Format("{0} {1}", syncItem.EpisodeTitle, e.Error.Message), e.Error);
                }
                else
                {
                    args = new StatusUpdateEventArgs(StatusUpdateEventArgs.Level.Status, string.Format("{0} Completed", syncItem.EpisodeTitle));
                    _fileUtilities.FileRename(GetDownloadFilename(), _syncItem.DestinationPath, true);

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
                            OnStatusUpdate(new StatusUpdateEventArgs(StatusUpdateEventArgs.Level.Warning, string.Format("{0}, cannot write to state file, will retry",syncItem.EpisodeTitle), null));
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

                OnStatusUpdate(args);

                _client.Dispose();
                _client = null;

                _complete = true;
                TaskComplete.Set();
            }
        }

        private void OnStatusUpdate(StatusUpdateEventArgs e)
        {
            if (StatusUpdate != null)
                StatusUpdate(this, e);
        }

        /// <summary>
        /// gets the display name for the task
        /// </summary>
        /// <returns></returns>
        public string GetName()
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
