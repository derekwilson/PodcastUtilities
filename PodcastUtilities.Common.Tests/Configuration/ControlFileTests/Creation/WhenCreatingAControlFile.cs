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
using System.Linq;
using NUnit.Framework;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Playlists;

namespace PodcastUtilities.Common.Tests.Configuration.ControlFileTests.Creation
{
    public class WhenCreatingAControlFile : WhenTestingAControlFile
    {
        protected override void When()
        {
            ControlFile = new ReadOnlyControlFile(ControlFileXmlDocument);
        }

        [Test]
        public void ItShouldCreateAnObject()
        {
            Assert.That(ControlFile, Is.Not.Null);
        }

        [Test]
        public void ItShouldGetThePodcasts()
        {
            Assert.That(ControlFile.GetPodcasts().Count(), Is.EqualTo(3));
        }

        [Test]
        public void ItShouldReadPodcast1()
        {
            Assert.That(ControlFile.GetPodcasts().ElementAt(0).Feed, Is.Null);
            Assert.That(ControlFile.GetPodcasts().ElementAt(0).Folder, Is.EqualTo("Test Match Special"));
            Assert.That(ControlFile.GetPodcasts().ElementAt(0).MaximumNumberOfFiles.Value, Is.EqualTo(987));
            Assert.That(ControlFile.GetPodcasts().ElementAt(0).Pattern.Value, Is.EqualTo("*.xyz"));
            Assert.That(ControlFile.GetPodcasts().ElementAt(0).SortField.Value, Is.EqualTo(PodcastFileSortField.FileName));
            Assert.That(ControlFile.GetPodcasts().ElementAt(0).AscendingSort.Value, Is.True);
            Assert.That(ControlFile.GetPodcasts().ElementAt(0).PostDownloadCommand, Is.Null);
            Assert.That(ControlFile.GetPodcasts().ElementAt(0).DeleteEmptyFolder.Value, Is.False);
        }

        [Test]
        public void ItShouldReadPodcast2()
        {
            Assert.That(ControlFile.GetPodcasts().ElementAt(1).Feed.Address.ToString(), Is.EqualTo("http://www.hanselminutes.com/hanselminutes_MP3Direct.xml"));
            Assert.That(ControlFile.GetPodcasts().ElementAt(1).Folder, Is.EqualTo("Hanselminutes"));
            Assert.That(ControlFile.GetPodcasts().ElementAt(1).MaximumNumberOfFiles.Value, Is.EqualTo(34));
            Assert.That(ControlFile.GetPodcasts().ElementAt(1).Pattern.Value, Is.EqualTo("*.mp3"));
            Assert.That(ControlFile.GetPodcasts().ElementAt(1).SortField.Value, Is.EqualTo(PodcastFileSortField.FileName));
            Assert.That(ControlFile.GetPodcasts().ElementAt(1).AscendingSort.Value, Is.False);
            Assert.That(ControlFile.GetPodcasts().ElementAt(1).PostDownloadCommand.Command.Value, Is.EqualTo("different command"));
            Assert.That(ControlFile.GetPodcasts().ElementAt(1).DeleteEmptyFolder.Value, Is.True);
        }

        [Test]
        public void ItShouldReadPodcast3()
        {
            Assert.That(ControlFile.GetPodcasts().ElementAt(2).PostDownloadCommand.Command.Value, Is.EqualTo("command"));
        }

        [Test]
        public void ItShouldReadTheGlobalInformation()
        {
            Assert.That(ControlFile.GetRetryWaitInSeconds(), Is.EqualTo(77));
            Assert.That(ControlFile.GetMaximumNumberOfConcurrentDownloads(), Is.EqualTo(10));
            Assert.That(ControlFile.GetSourceRoot(), Is.EqualTo(".\\profile\\iPodder\\downloads"));
            Assert.That(ControlFile.GetDestinationRoot(),Is.EqualTo("W:\\Podcasts"));
            Assert.That(ControlFile.GetPlaylistFileName(),Is.EqualTo("podcasts.wpl"));
            Assert.That(ControlFile.GetFreeSpaceToLeaveOnDestination(), Is.EqualTo(2000));
            Assert.That(ControlFile.GetFreeSpaceToLeaveOnDownload(), Is.EqualTo(3000));
            Assert.That(ControlFile.GetPlaylistFormat(), Is.EqualTo(PlaylistFormat.WPL));
        }
    }
}
