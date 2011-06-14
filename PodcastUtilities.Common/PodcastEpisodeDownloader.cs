using System;
using System.ComponentModel;
using System.Net;
using System.Threading;
using PodcastUtilities.Common.IO;

namespace PodcastUtilities.Common
{
    /// <summary>
    /// a task that will download a podcast episode on a background thread
    /// </summary>
    public class PodcastEpisodeDownloader : IPodcastEpisodeDownloader
    {
        private readonly IWebClientFactory _webClientFactory;

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
        /// the event that is fired when the task completes
        /// </summary>
        public EventWaitHandle TaskComplete { get; protected set; }

        /// <summary>
        /// create a task
        /// </summary>
        /// <param name="webClientFactory"></param>
        public PodcastEpisodeDownloader(IWebClientFactory webClientFactory)
        {
            _webClientFactory = webClientFactory;
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
                _client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                _client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
                _client.DownloadFileAsync(SyncItem.EpisodeUrl, SyncItem.DestinationPath, SyncItem);
            }
        }

        void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            StatusUpdateEventArgs args = null;
            lock (_lock)
            {
                Exception exception = e.UserState as Exception;
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
                    args = new StatusUpdateEventArgs(StatusUpdateEventArgs.Level.Error, e.Error.InnerException.Message, e.Error.InnerException);
                }
                else if (e.Error != null)
                {
                    args = new StatusUpdateEventArgs(StatusUpdateEventArgs.Level.Error, e.Error.Message, e.Error);
                }
                else
                {
                    args = new StatusUpdateEventArgs(StatusUpdateEventArgs.Level.Status, string.Format("{0} Completed", syncItem.EpisodeTitle));
                }

                OnStatusUpdate(args);

                _client.Dispose();
                _client = null;
                _complete = true;
                TaskComplete.Set();
            }
        }

        void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            lock (_lock)
            {
                if (_progressPercentage == e.ProgressPercentage)
                {
                    return;
                }
                _progressPercentage = e.ProgressPercentage;
            }

            StatusUpdateEventArgs args = null;
            args = new StatusUpdateEventArgs(StatusUpdateEventArgs.Level.Progress, e.ProgressPercentage.ToString());
            OnStatusUpdate(args);
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
