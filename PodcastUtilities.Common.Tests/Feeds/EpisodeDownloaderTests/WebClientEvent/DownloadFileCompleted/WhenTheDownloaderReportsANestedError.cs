using System;
using System.ComponentModel;
using NUnit.Framework;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.Feeds.EpisodeDownloaderTests.WebClientEvent.DownloadFileCompleted
{
    public class WhenTheDownloaderReportsANestedError : WhenTestingTheDownloaderCompletedMechanism
    {
        protected override void When()
        {
            _webClient.Raise(client => client.DownloadFileCompleted += null, this,
                             new AsyncCompletedEventArgs(new Exception("OUTER ERROR", _reportedError), false, _syncItem));
        }

        [Test]
        public void ItShouldComplete()
        {
            Assert.That(_downloader.IsStarted(), Is.True);
            Assert.That(_downloader.IsComplete(), Is.True);
        }

        [Test]
        public void ItShouldSendTheCorrectStatus()
        {
            Assert.That(_statusUpdateArgs.Exception, Is.SameAs(_reportedError));
            Assert.That(_statusUpdateArgs.MessageLevel, Is.EqualTo(StatusUpdateLevel.Error));
            Assert.That(_statusUpdateArgs.Message, Is.StringContaining("TEST ERROR"));
        }

        [Test]
        public void ItShouldTidyUp()
        {
            _webClient.AssertWasCalled(client => client.Dispose());
        }
    }
}