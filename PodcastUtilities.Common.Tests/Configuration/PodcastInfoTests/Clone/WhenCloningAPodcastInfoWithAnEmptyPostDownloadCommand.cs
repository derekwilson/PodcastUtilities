using NUnit.Framework;
using PodcastUtilities.Common.Configuration;

namespace PodcastUtilities.Common.Tests.Configuration.PodcastInfoTests.Clone
{
    public class WhenCloningAPodcastInfoWithAnEmptyPostDownloadCommand : WhenCloningAPodcastInfo
    {
        protected override void GivenThat()
        {
            base.GivenThat();
            _pocastInfo.AscendingSort.Value = true;
            _pocastInfo.Folder = "FOLDER";
            _pocastInfo.MaximumNumberOfFiles.Value = 123;
            _pocastInfo.Pattern.Value = "PATTERN";
            _pocastInfo.SortField.Value = PodcastFileSortField.FileName;

            _pocastInfo.CreatePostDownloadCommand();
        }

        protected override void When()
        {
            _clonedPodcast = _pocastInfo.Clone() as PodcastInfo;
        }

        [Test]
        public void ItShouldCloneThePostDownloadCommand()
        {
            Assert.That(_clonedPodcast.PostDownloadCommand, Is.Not.Null);
        }

        [Test]
        public void ItShouldCloneThePostDownloadCommandCommand()
        {
            Assert.That(_clonedPodcast.PostDownloadCommand.Command.IsSet, Is.EqualTo(false));
        }

        [Test]
        public void ItShouldCloneThePostDownloadCommandArguments()
        {
            Assert.That(_clonedPodcast.PostDownloadCommand.Arguments.IsSet, Is.EqualTo(false));
        }

        [Test]
        public void ItShouldCloneThePostDownloadCommandWorkingDirectory()
        {
            Assert.That(_clonedPodcast.PostDownloadCommand.WorkingDirectory.IsSet, Is.EqualTo(false));
        }
    }
}