﻿using NUnit.Framework;
using PodcastUtilities.Common.Configuration;

namespace PodcastUtilities.Common.Tests.Configuration.PodcastInfoTests.Clone
{
    public class WhenCloningAPodcastInfoWithoutOptionalElements : WhenCloningAPodcastInfo
    {
        protected override void GivenThat()
        {
            base.GivenThat();
            _pocastInfo.AscendingSort.Value = true;
            _pocastInfo.Folder = "FOLDER";
            _pocastInfo.MaximumNumberOfFiles.Value = 123;
            _pocastInfo.Pattern.Value = "PATTERN";
            _pocastInfo.SortField.Value = PodcastFileSortField.FileName;
        }

        protected override void When()
        {
            _clonedPodcast = _pocastInfo.Clone() as PodcastInfo;
        }

        [Test]
        public void ItShouldCloneThePodcastSortDirection()
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
        public void ItShouldCloneThePodcastPattern()
        {
            Assert.That(_clonedPodcast.Pattern.Value, Is.EqualTo("PATTERN"));
        }

        [Test]
        public void ItShouldCloneThePodcastSortField()
        {
            Assert.That(_clonedPodcast.SortField.Value, Is.EqualTo(PodcastFileSortField.FileName));
        }

        [Test]
        public void ItShouldCloneThePostDownloadCommand()
        {
            Assert.That(_clonedPodcast.PostDownloadCommand, Is.Null);
        }

        [Test]
        public void ItShouldCloneTheFeed()
        {
            Assert.That(_clonedPodcast.Feed, Is.Null);
        }

    }
}