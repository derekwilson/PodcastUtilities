namespace PodcastUtilities.Common.Tests.Feeds.EpisodeDownloaderTests.WebClientEvent.DownloadProgressChanged
{
    public abstract class WhenTestingTheDownloaderProgressChangedMechanism : WhenTestingTheDownloader
    {
        protected ProgressEventArgs _progressEventArgs;
        protected long _bytesReceived;
        protected long _totalBytes;
        protected int _percentage;

        protected override void GivenThat()
        {
            base.GivenThat();

            _downloader.SyncItem = _syncItem;
            _downloader.Start(null);
        }
    }
}