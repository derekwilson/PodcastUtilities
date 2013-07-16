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


namespace PodcastUtilities.Common.Tests.Configuration.ControlFileTests.Clone
{
    public class WhenCloningAControlFileAfterSettingProperties : WhenCloningAControlFile
    {
        protected override void GivenThat()
        {
            base.GivenThat();

            _controlFile.SetSourceRoot("SOURCE");
            _controlFile.SetDestinationRoot("DESTINATION");
            _controlFile.SetDiagnosticOutput(DiagnosticOutputLevel.None);
            _controlFile.SetDiagnosticRetainTemporaryFiles(false);
            _controlFile.SetFreeSpaceToLeaveOnDestination(135);
            _controlFile.SetFreeSpaceToLeaveOnDownload(357);
            _controlFile.SetMaximumNumberOfConcurrentDownloads(579);
            _controlFile.SetPlaylistFileName("PLAYLIST");
            _controlFile.SetPlaylistFormat(PlaylistFormat.Unknown);
            _controlFile.SetRetryWaitInSeconds(7911);

            _controlFile.SetDefaultAscendingSort(false);
            _controlFile.SetDefaultDeleteDownloadsDaysOld(246);
            _controlFile.SetDefaultDownloadStrategy(PodcastEpisodeDownloadStrategy.All);
            _controlFile.SetDefaultFeedFormat(PodcastFeedFormat.ATOM);
            _controlFile.SetDefaultFilePattern("PATTERN");
            _controlFile.SetDefaultMaximumDaysOld(468);
            _controlFile.SetDefaultNamingStyle(PodcastEpisodeNamingStyle.UrlFileName);
            _controlFile.SetDefaultNumberOfFiles(680);
            _controlFile.SetDefaultSortField(PodcastFileSortField.CreationTime);
            _controlFile.SetDefaultPostDownloadCommand("COMMAND");
            _controlFile.SetDefaultPostDownloadArguments("ARGS");
            _controlFile.SetDefaultPostDownloadWorkingDirectory("CWD");
        }

        protected override void When()
        {
            _clonedControlFile = _controlFile.Clone() as ReadWriteControlFile;
        }

        // we only need to test the global section as the podcasts are tested by their close tests

        [Test]
        public void ItShouldCloneTheSourceRoot()
        {
            Assert.That(_clonedControlFile.GetSourceRoot(), Is.EqualTo("SOURCE"));
        }

        [Test]
        public void ItShouldCloneTheDestinationRoot()
        {
            Assert.That(_clonedControlFile.GetDestinationRoot(), Is.EqualTo("DESTINATION"));
        }

        [Test]
        public void ItShouldCloneThePlaylistFilename()
        {
            Assert.That(_clonedControlFile.GetPlaylistFileName(), Is.EqualTo("PLAYLIST"));
        }

        [Test]
        public void ItShouldCloneThePlaylistFormat()
        {
            Assert.That(_clonedControlFile.GetPlaylistFormat(), Is.EqualTo(PlaylistFormat.Unknown));
        }

        [Test]
        public void ItShouldCloneTheFreeSpaceOnDest()
        {
            Assert.That(_clonedControlFile.GetFreeSpaceToLeaveOnDestination(), Is.EqualTo(135));
        }

        [Test]
        public void ItShouldCloneTheFreeSpaceOnSource()
        {
            Assert.That(_clonedControlFile.GetFreeSpaceToLeaveOnDownload(), Is.EqualTo(357));
        }

        [Test]
        public void ItShouldCloneTheNumberOfConcurrentDownloads()
        {
            Assert.That(_clonedControlFile.GetMaximumNumberOfConcurrentDownloads(), Is.EqualTo(579));
        }

        [Test]
        public void ItShouldCloneTheRetryWait()
        {
            Assert.That(_clonedControlFile.GetRetryWaitInSeconds(), Is.EqualTo(7911));
        }

        [Test]
        public void ItShouldCloneTheDiagOutput()
        {
            Assert.That(_clonedControlFile.GetDiagnosticOutput(), Is.EqualTo(DiagnosticOutputLevel.None));
        }

        [Test]
        public void ItShouldCloneTheDiagRetailFiles()
        {
            Assert.That(_clonedControlFile.GetDiagnosticRetainTemporaryFiles(), Is.EqualTo(false));
        }

        [Test]
        public void ItShouldCloneTheDefaultDeleteDaysOld()
        {
            Assert.That(_clonedControlFile.GetDefaultDeleteDownloadsDaysOld(), Is.EqualTo(246));
        }

        [Test]
        public void ItShouldCloneTheDefaultMaximumDaysOld()
        {
            Assert.That(_clonedControlFile.GetDefaultMaximumDaysOld(), Is.EqualTo(468));
        }

        [Test]
        public void ItShouldCloneTheDefaultNumber()
        {
            Assert.That(_clonedControlFile.GetDefaultNumberOfFiles(), Is.EqualTo(680));
        }

        [Test]
        public void ItShouldCloneTheDefaultPattern()
        {
            Assert.That(_clonedControlFile.GetDefaultFilePattern(), Is.EqualTo("PATTERN"));
        }

        [Test]
        public void ItShouldCloneTheDefaultSortField()
        {
            Assert.That(_clonedControlFile.GetDefaultSortField(), Is.EqualTo(PodcastFileSortField.CreationTime));
        }

        [Test]
        public void ItShouldCloneTheDefaultSortDirection()
        {
            Assert.That(_clonedControlFile.GetDefaultAscendingSort(), Is.EqualTo(false));
        }

        [Test]
        public void ItShouldCloneTheDefaultPostDownloadCommand()
        {
            Assert.That(_clonedControlFile.GetDefaultPostDownloadCommand(), Is.EqualTo("COMMAND"));
        }

        [Test]
        public void ItShouldCloneTheDefaultPostDownloadArguments()
        {
            Assert.That(_clonedControlFile.GetDefaultPostDownloadArguments(), Is.EqualTo("ARGS"));
        }

        [Test]
        public void ItShouldCloneTheDefaultPostDownloadWorkingDirectory()
        {
            Assert.That(_clonedControlFile.GetDefaultPostDownloadWorkingDirectory(), Is.EqualTo("CWD"));
        }

        [Test]
        public void ItShouldCloneThePodcasts()
        {
            Assert.That(_clonedControlFile.GetPodcasts().Count(), Is.EqualTo(3));
        }

    }
}