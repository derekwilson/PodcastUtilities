﻿#region License
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
using System;
using System.ComponentModel;
using System.Drawing;
using Moq;
using NUnit.Framework;

namespace PodcastUtilities.Common.Multiplatform.Tests.Feeds.EpisodeDownloaderTests.WebClientEvent.DownloadFileCompleted
{
    public class WhenCompletingTheDownloaderWithTheStateFileLocked : WhenTestingTheDownloaderCompletedMechanism
    {
        protected override void SetupStubs()
        {
            base.SetupStubs();
            int callCount = 0;
            _state.Setup(s => s.SaveState(_downloadFolder)).Callback(() =>
                {
                    callCount++;
                    if (callCount == 1)
                        throw new System.IO.IOException();
                });
        }

        protected override void When()
        {
            _webClient.Raise(client => client.DownloadFileCompleted += null, new AsyncCompletedEventArgs(null, false, _syncItem));
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
            Assert.That(_statusUpdateArgs.Exception, Is.Null);
            Assert.That(_statusUpdateArgs.MessageLevel, Is.EqualTo(StatusUpdateLevel.Status));
            StringAssert.Contains("Completed", _statusUpdateArgs.Message);
            Assert.That(_statusUpdateArgs.IsTaskCompletedSuccessfully, Is.True);
            Assert.That(_statusUpdateArgs.UserState, Is.SameAs(_syncItem));
        }

        [Test]
        public void ItShouldTidyUp()
        {
            _webClient.Verify(client => client.Dispose());
        }

        [Test]
        public void ItShouldRenameTheDownload()
        {
            _fileUtilities.Verify(utils => utils.FileRename("c:\\folder\\file.partial", _syncItem.DestinationPath, true));
        }

        [Test]
        public void ItShouldUpdateTheState()
        {
            _state.VerifySet(state => state.DownloadHighTide = _published, Times.Exactly(2));
            _state.Verify(state => state.SaveState(_downloadFolder), Times.Exactly(2));
        }
    }
}