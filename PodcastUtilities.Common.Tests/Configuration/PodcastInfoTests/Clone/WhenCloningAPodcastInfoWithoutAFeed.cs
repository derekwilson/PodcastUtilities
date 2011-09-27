using NUnit.Framework;
using PodcastUtilities.Common.Configuration;

namespace PodcastUtilities.Common.Tests.Configuration.PodcastInfoTests.Clone
{
    public class WhenCloningAPodcastInfoWithoutAFeed : WhenCloningAPodcastInfo
    {
        protected override void GivenThat()
        {
            base.GivenThat();
            _pocastInfo.AscendingSort.Value = true;
            _pocastInfo.Folder = "FOLDER";
            _pocastInfo.MaximumNumberOfFiles = 123;
            _pocastInfo.Pattern = "PATTERN";
            _pocastInfo.SortField.Value = PodcastFileSortField.FileName;
        }

        protected override void When()
        {
            _clonedPodcast = _pocastInfo.Clone() as PodcastInfo;
        }

        [Test]
        public void ItShouldCloneThePodcast()
        {
            Assert.That(_clonedPodcast.AscendingSort.Value, Is.EqualTo(true));
            Assert.That(_clonedPodcast.Folder, Is.EqualTo("FOLDER"));
            Assert.That(_clonedPodcast.MaximumNumberOfFiles, Is.EqualTo(123));
            Assert.That(_clonedPodcast.Pattern, Is.EqualTo("PATTERN"));
            Assert.That(_clonedPodcast.SortField.Value, Is.EqualTo(PodcastFileSortField.FileName));
        }
    }
}