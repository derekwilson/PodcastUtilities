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
            Assert.That(_controlFile.GetPodcasts().ElementAt(0).MaximumNumberOfFiles, Is.EqualTo(-1));
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
        public void ItShouldReadPodcast2()
        {
            Assert.That(_controlFile.GetPodcasts().ElementAt(1).Folder, Is.EqualTo("Hanselminutes"));
            Assert.That(_controlFile.GetPodcasts().ElementAt(1).MaximumNumberOfFiles, Is.EqualTo(34));
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
            Assert.That(_controlFile.GetPodcasts().ElementAt(1).Feed.DownloadStrategy.Value, Is.EqualTo(PodcastEpisodeDownloadStrategy.Latest));

            Assert.That(_controlFile.GetPodcasts().ElementAt(1).Feed.NamingStyle.IsSet, Is.EqualTo(false));
            Assert.That(_controlFile.GetPodcasts().ElementAt(1).Feed.MaximumDaysOld.IsSet, Is.EqualTo(false));
            Assert.That(_controlFile.GetPodcasts().ElementAt(1).Feed.DeleteDownloadsDaysOld.IsSet, Is.EqualTo(false));
            Assert.That(_controlFile.GetPodcasts().ElementAt(1).Feed.DownloadStrategy.IsSet, Is.EqualTo(false));
        }

        [Test]
        public void ItShouldReadTheGlobalRetryWaitInSeconds()
        {
            Assert.That(_controlFile.GetRetryWaitInSeconds(), Is.EqualTo(77));
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

    }
}