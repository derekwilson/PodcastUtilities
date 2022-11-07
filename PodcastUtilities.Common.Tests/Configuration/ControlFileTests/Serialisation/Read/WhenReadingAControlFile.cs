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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using NUnit.Framework;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Playlists;

namespace PodcastUtilities.Common.Tests.Configuration.ControlFileTests.Serialisation.Read
{
    public class WhenReadingAControlFile
        : WhenTestingBehaviour
    {
        protected ReadOnlyControlFile _controlFile;

        protected XmlReader _xmlReader;
        private string _testControlFileResourcePath;

        protected override void GivenThat()
        {
            base.GivenThat();

            _testControlFileResourcePath = "PodcastUtilities.Common.Tests.XML.testcontrolfile.xml";
            Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream(_testControlFileResourcePath);

            XmlReaderSettings readSettings = new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Document };
            _xmlReader = XmlReader.Create(s, readSettings);

            _controlFile = new ReadOnlyControlFile();
        }

        protected override void When()
        {
            _controlFile.ReadXml(_xmlReader);
        }

        [Test]
        public void ItShouldCreateAnObject()
        {
            Assert.That(_controlFile, Is.Not.Null);
        }

        [Test]
        public void ItShouldGetThePodcasts()
        {
            Assert.That(_controlFile.GetPodcasts().Count(), Is.EqualTo(3));
        }

        [Test]
        public void ItShouldReadPodcast1()
        {
            Assert.That(_controlFile.GetPodcasts().ElementAt(0).Folder, Is.EqualTo("Test Match Special"));
            Assert.That(_controlFile.GetPodcasts().ElementAt(0).MaximumNumberOfFiles.Value, Is.EqualTo(987));
            Assert.That(_controlFile.GetPodcasts().ElementAt(0).Pattern.Value, Is.EqualTo("*.xyz"));
            Assert.That(_controlFile.GetPodcasts().ElementAt(0).SortField.Value, Is.EqualTo(PodcastFileSortField.FileName));
            Assert.That(_controlFile.GetPodcasts().ElementAt(0).AscendingSort.Value, Is.True);
        }

        [Test]
        public void ItShouldReadPodcast1Feed()
        {
            Assert.That(_controlFile.GetPodcasts().ElementAt(0).Feed, Is.Null);
        }

        [Test]
        public void ItShouldReadPodcast1PostDownloadCommand()
        {
            Assert.That(_controlFile.GetPodcasts().ElementAt(0).PostDownloadCommand, Is.Null);
        }

        [Test]
        public void ItShouldReadPodcast2()
        {
            Assert.That(_controlFile.GetPodcasts().ElementAt(1).Folder, Is.EqualTo("Hanselminutes"));
            Assert.That(_controlFile.GetPodcasts().ElementAt(1).MaximumNumberOfFiles.Value, Is.EqualTo(34));
            Assert.That(_controlFile.GetPodcasts().ElementAt(1).Pattern.Value, Is.EqualTo("*.mp3"));
            Assert.That(_controlFile.GetPodcasts().ElementAt(1).SortField.Value, Is.EqualTo(PodcastFileSortField.FileName));
            Assert.That(_controlFile.GetPodcasts().ElementAt(1).AscendingSort.Value, Is.False);
        }

        [Test]
        public void ItShouldReadPodcast2Feed()
        {
            Assert.That(_controlFile.GetPodcasts().ElementAt(1).Feed.Address.ToString(), Is.EqualTo("http://www.hanselminutes.com/hanselminutes_MP3Direct.xml"));
            Assert.That(_controlFile.GetPodcasts().ElementAt(1).Feed.NamingStyle.Value, Is.EqualTo(PodcastEpisodeNamingStyle.UrlFileNameAndPublishDateTime));
            Assert.That(_controlFile.GetPodcasts().ElementAt(1).Feed.MaximumDaysOld.Value, Is.EqualTo(99));
            Assert.That(_controlFile.GetPodcasts().ElementAt(1).Feed.DeleteDownloadsDaysOld.Value, Is.EqualTo(88));
            Assert.That(_controlFile.GetPodcasts().ElementAt(1).Feed.MaximumNumberOfDownloadedItems.Value, Is.EqualTo(55));
            Assert.That(_controlFile.GetPodcasts().ElementAt(1).Feed.DownloadStrategy.Value, Is.EqualTo(PodcastEpisodeDownloadStrategy.Latest));

            Assert.That(_controlFile.GetPodcasts().ElementAt(1).Feed.NamingStyle.IsSet, Is.EqualTo(false));
            Assert.That(_controlFile.GetPodcasts().ElementAt(1).Feed.MaximumDaysOld.IsSet, Is.EqualTo(false));
            Assert.That(_controlFile.GetPodcasts().ElementAt(1).Feed.DeleteDownloadsDaysOld.IsSet, Is.EqualTo(false));
            Assert.That(_controlFile.GetPodcasts().ElementAt(1).Feed.MaximumNumberOfDownloadedItems.IsSet, Is.EqualTo(false));
            Assert.That(_controlFile.GetPodcasts().ElementAt(1).Feed.DownloadStrategy.IsSet, Is.EqualTo(false));
        }

        [Test]
        public void ItShouldReadPodcast2PostDownloadCommand()
        {
            Assert.That(_controlFile.GetPodcasts().ElementAt(1).PostDownloadCommand.Command.Value, Is.EqualTo("different command"));
        }

        [Test]
        public void ItShouldReadPodcast3PostDownloadCommand()
        {
            Assert.That(_controlFile.GetPodcasts().ElementAt(2).PostDownloadCommand.Command.Value, Is.EqualTo("command"));
        }

        [Test]
        public void ItShouldReadPodcast3Feed()
        {
            Assert.That(_controlFile.GetPodcasts().ElementAt(2).Feed.Address.ToString(), Is.EqualTo("http://www.hanselminutes.com/hanselminutes_MP3Direct.xml"));
            Assert.That(_controlFile.GetPodcasts().ElementAt(2).Feed.NamingStyle.Value, Is.EqualTo(PodcastEpisodeNamingStyle.UrlFileNameFeedTitleAndPublishDateTime));
            Assert.That(_controlFile.GetPodcasts().ElementAt(2).Feed.MaximumDaysOld.Value, Is.EqualTo(11));
            Assert.That(_controlFile.GetPodcasts().ElementAt(2).Feed.DeleteDownloadsDaysOld.Value, Is.EqualTo(14));
            Assert.That(_controlFile.GetPodcasts().ElementAt(2).Feed.MaximumNumberOfDownloadedItems.Value, Is.EqualTo(15));
            Assert.That(_controlFile.GetPodcasts().ElementAt(2).Feed.DownloadStrategy.Value, Is.EqualTo(PodcastEpisodeDownloadStrategy.HighTide));

            Assert.That(_controlFile.GetPodcasts().ElementAt(2).Feed.NamingStyle.IsSet, Is.EqualTo(true));
            Assert.That(_controlFile.GetPodcasts().ElementAt(2).Feed.MaximumDaysOld.IsSet, Is.EqualTo(true));
            Assert.That(_controlFile.GetPodcasts().ElementAt(2).Feed.DeleteDownloadsDaysOld.IsSet, Is.EqualTo(true));
            Assert.That(_controlFile.GetPodcasts().ElementAt(2).Feed.MaximumNumberOfDownloadedItems.IsSet, Is.EqualTo(true));
            Assert.That(_controlFile.GetPodcasts().ElementAt(2).Feed.DownloadStrategy.IsSet, Is.EqualTo(true));
        }

        [Test]
        public void ItShouldReadTheDiagnosticOutputLevel()
        {
            Assert.That(_controlFile.GetDiagnosticOutput(), Is.EqualTo(DiagnosticOutputLevel.Verbose));
        }

        [Test]
        public void ItShouldReadTheDiagnosticRetainFilesFlag()
        {
            Assert.That(_controlFile.GetDiagnosticRetainTemporaryFiles(), Is.EqualTo(true));
        }

        [Test]
        public void ItShouldReadTheGlobalMaximumNumberOfConcurrentDownloads()
        {
            Assert.That(_controlFile.GetMaximumNumberOfConcurrentDownloads(), Is.EqualTo(10));
        }

        [Test]
        public void ItShouldReadTheGlobalSourceRoot()
        {
            Assert.That(_controlFile.GetSourceRoot(), Is.EqualTo(".\\profile\\iPodder\\downloads"));
        }

        [Test]
        public void ItShouldReadTheGlobalDestinationRoot()
        {
            Assert.That(_controlFile.GetDestinationRoot(), Is.EqualTo("W:\\Podcasts"));
        }

        [Test]
        public void ItShouldReadTheGlobalPlaylistFileName()
        {
            Assert.That(_controlFile.GetPlaylistFileName(), Is.EqualTo("podcasts.wpl"));
        }

        [Test]
        public void ItShouldReadTheGlobalFreeSpaceToLeaveOnDestination()
        {
            Assert.That(_controlFile.GetFreeSpaceToLeaveOnDestination(), Is.EqualTo(2000));
        }

        [Test]
        public void ItShouldReadTheGlobalFreeSpaceToLeaveOnDownload()
        {
            Assert.That(_controlFile.GetFreeSpaceToLeaveOnDownload(), Is.EqualTo(3000));
        }

        [Test]
		public void ItShouldReadTheGlobalPlaylistFormat()
		{
			Assert.That(_controlFile.GetPlaylistFormat(), Is.EqualTo(PlaylistFormat.WPL));
		}

		[Test]
		public void ItShouldReadTheGlobalPlaylistSeparator()
		{
			Assert.That(_controlFile.GetPlaylistPathSeparator(), Is.EqualTo("SEP"));
		}

	}
}