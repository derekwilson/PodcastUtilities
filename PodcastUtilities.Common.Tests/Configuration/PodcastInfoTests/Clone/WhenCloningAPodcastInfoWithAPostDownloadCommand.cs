using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PodcastUtilities.Common.Configuration;

namespace PodcastUtilities.Common.Tests.Configuration.PodcastInfoTests.Clone
{
    public class WhenCloningAPodcastInfoWithAPostDownloadCommand : WhenCloningAPodcastInfo
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
            _pocastInfo.PostDownloadCommand.Command.Value = "CMD";
            _pocastInfo.PostDownloadCommand.Arguments.Value = "ARGS";
            _pocastInfo.PostDownloadCommand.WorkingDirectory.Value = "CWD";
        }

        protected override void When()
        {
            _clonedPodcast = _pocastInfo.Clone() as PodcastInfo;
        }

        [Test]
        public void ItShouldCloneThePodcastAscendingSort()
        {
            Assert.That(_clonedPodcast.AscendingSort.Value, Is.EqualTo(true));
        }

        [Test]
        public void ItShouldCloneThePodcastFolder()
        {
            Assert.That(_clonedPodcast.Folder, Is.EqualTo("FOLDER"));
        }

        [Test]
        public void ItShouldCloneThePodcastMaximumNumberOfFiles()
        {
            Assert.That(_clonedPodcast.MaximumNumberOfFiles.Value, Is.EqualTo(123));
        }

        [Test]
        public void ItShouldCloneThePodcastSortField()
        {
            Assert.That(_clonedPodcast.SortField.Value, Is.EqualTo(PodcastFileSortField.FileName));
        }

        [Test]
        public void ItShouldCloneThePostDownloadCommand()
        {
            Assert.That(_clonedPodcast.PostDownloadCommand.Command.Value, Is.EqualTo("CMD"));
        }

        [Test]
        public void ItShouldCloneThePostDownloadCommandArgs()
        {
            Assert.That(_clonedPodcast.PostDownloadCommand.Arguments.Value, Is.EqualTo("ARGS"));
        }

        [Test]
        public void ItShouldCloneThePostDownloadCommandWorkingDir()
        {
            Assert.That(_clonedPodcast.PostDownloadCommand.WorkingDirectory.Value, Is.EqualTo("CWD"));
        }

    }
}
