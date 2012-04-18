using NUnit.Framework;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Playlists;

namespace PodcastUtilities.Common.Tests.Configuration.ControlFileTests.Clone
{
    public class WhenCloningTheTestControlFile : WhenCloningAControlFile
    {
        protected override void When()
        {
            _clonedControlFile = _controlFile.Clone() as ReadWriteControlFile;
        }

        // we only need to test the global section as the podcasts are tested by their close tests

        [Test]
        public void ItShouldCloneTheSourceRoot()
        {
            Assert.That(_clonedControlFile.GetSourceRoot(), Is.EqualTo(@".\profile\iPodder\downloads"));
        }

        [Test]
        public void ItShouldCloneTheDestinationRoot()
        {
            Assert.That(_clonedControlFile.GetDestinationRoot(), Is.EqualTo(@"W:\Podcasts"));
        }

        [Test]
        public void ItShouldCloneThePlaylistFilename()
        {
            Assert.That(_clonedControlFile.GetPlaylistFileName(), Is.EqualTo(@"podcasts.wpl"));
        }

        [Test]
        public void ItShouldCloneThePlaylistFormat()
        {
            Assert.That(_clonedControlFile.GetPlaylistFormat(), Is.EqualTo(PlaylistFormat.WPL));
        }

        [Test]
        public void ItShouldCloneTheFreeSpaceOnDest()
        {
            Assert.That(_clonedControlFile.GetFreeSpaceToLeaveOnDestination(), Is.EqualTo(2000));
        }

        [Test]
        public void ItShouldCloneTheFreeSpaceOnSource()
        {
            Assert.That(_clonedControlFile.GetFreeSpaceToLeaveOnDownload(), Is.EqualTo(3000));
        }

        [Test]
        public void ItShouldCloneTheDefaultNumber()
        {
            Assert.That(_clonedControlFile.GetDefaultNumberOfFiles(), Is.EqualTo(987));
        }

        [Test]
        public void ItShouldCloneTheDefaultPattern()
        {
            Assert.That(_clonedControlFile.GetDefaultFilePattern(), Is.EqualTo("*.xyz"));
        }

        [Test]
        public void ItShouldCloneTheDefaultSortField()
        {
            Assert.That(_clonedControlFile.GetDefaultSortField(), Is.EqualTo(PodcastFileSortField.FileName));
        }

        [Test]
        public void ItShouldCloneTheDefaultSortDirection()
        {
            Assert.That(_clonedControlFile.GetDefaultAscendingSort(), Is.EqualTo(true));
        }

        [Test]
        public void ItShouldCloneTheNumberOfConcurrentDownloads()
        {
            Assert.That(_clonedControlFile.GetMaximumNumberOfConcurrentDownloads(), Is.EqualTo(10));
        }

        [Test]
        public void ItShouldCloneTheRetryWait()
        {
            Assert.That(_clonedControlFile.GetRetryWaitInSeconds(), Is.EqualTo(77));
        }

        [Test]
        public void ItShouldCloneTheDiagOutput()
        {
            Assert.That(_clonedControlFile.GetDiagnosticOutput(), Is.EqualTo(DiagnosticOutputLevel.Verbose));
        }

        [Test]
        public void ItShouldCloneTheDiagRetailFiles()
        {
            Assert.That(_clonedControlFile.GetDiagnosticRetainTemporaryFiles(), Is.EqualTo(true));
        }
    }
}