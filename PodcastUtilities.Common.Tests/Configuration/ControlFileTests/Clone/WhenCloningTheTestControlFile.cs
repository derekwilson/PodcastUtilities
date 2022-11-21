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
		public void ItShouldCloneThePlaylistSeparator()
		{
			Assert.That(_clonedControlFile.GetPlaylistPathSeparator(), Is.EqualTo("SEP"));
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
        public void ItShouldCloneTheDefaultDeleteDaysOld()
        {
            Assert.That(_clonedControlFile.GetDefaultDeleteDownloadsDaysOld(), Is.EqualTo(88));
        }

        [Test]
        public void ItShouldCloneTheDefaultMaximumNumberOfDownloadedItems()
        {
            Assert.That(_clonedControlFile.GetDefaultMaximumNumberOfDownloadedItems(), Is.EqualTo(55));
        }

        [Test]
        public void ItShouldCloneTheDefaultMaximumDaysOld()
        {
            Assert.That(_clonedControlFile.GetDefaultMaximumDaysOld(), Is.EqualTo(99));
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

        [Test]
        public void ItShouldCloneThePodcasts()
        {
            Assert.That(_clonedControlFile.GetPodcasts().Count(), Is.EqualTo(3));
        }

    }
}

