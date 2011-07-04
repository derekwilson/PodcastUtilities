using System;
using System.ComponentModel;
using System.Net;
using System.Reflection;

namespace PodcastUtilities.Common.Tests.PodcastEpisodeDownloaderTests.WebClientEvent.DownloadProgressChanged
{
    public abstract class WhenTestingTheDownloaderProgressChangedMechanism : WhenTestingTheDownloader
    {
        protected DownloadProgressEventArgs _progressEventArgs;
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