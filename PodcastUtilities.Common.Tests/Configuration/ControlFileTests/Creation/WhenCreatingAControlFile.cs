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
