using System;

namespace PodcastUtilities.Common.Tests.PodcastEpisodeDownloaderTests.WebClientEvent.DownloadFileCompleted
{
    public abstract class WhenTestingTheDownloaderCompletedMechanism : WhenTestingTheDownloader
    {
        protected System.Exception _reportedError;

        protected override void GivenThat()
        {
            base.GivenThat();

            _reportedError = new Exception("TEST ERROR");

            _downloader.SyncItem = _syncItem;
            _downloader.Start(null);
        }
    }
}