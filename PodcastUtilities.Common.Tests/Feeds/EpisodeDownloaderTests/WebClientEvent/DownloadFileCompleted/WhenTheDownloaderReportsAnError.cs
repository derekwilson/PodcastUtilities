#region License
// FreeBSD License
// Copyright (c) 2010 - 2013, Andrew Trevarrow and Derek Wilson
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
// Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
// 
// Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED 
// WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A 
// PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR
// ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
// TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
// POSSIBILITY OF SUCH DAMAGE.
#endregion
using System.ComponentModel;
using NUnit.Framework;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.Feeds.EpisodeDownloaderTests.WebClientEvent.DownloadFileCompleted
{
    public class WhenTheDownloaderReportsAnError : WhenTestingTheDownloaderCompletedMechanism
    {
        protected override void When()
        {
            _webClient.Raise(client => client.DownloadFileCompleted += null, this,
                             new AsyncCompletedEventArgs(_reportedError, false, _syncItem));
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
            Assert.That(_statusUpdateArgs.Message, Is.StringContaining("Error in: title"));     // error should show the episode title
            Assert.That(_statusUpdateArgs.IsTaskCompletedSuccessfully, Is.False);
            Assert.That(_statusUpdateArgs.UserState, Is.SameAs(_syncItem));
        }

        [Test]
        public void ItShouldTidyUp()
        {
            _webClient.AssertWasCalled(client => client.Dispose());
        }
    }
}