using FakeItEasy;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Playlists;
using System.Collections.Generic;

namespace PodcastUtilities.AndroidTests.Helpers
{
    public class ControlFileMocker
    {
        private IReadWriteControlFile MockControlFile = A.Fake<IReadWriteControlFile>();

        public IReadWriteControlFile GetMockedControlFile() => MockControlFile;

        public ControlFileMocker ApplySourceRoot(string root)
        {
            A.CallTo(() => MockControlFile.GetSourceRoot()).Returns(root);
            return this;
        }

        public ControlFileMocker ApplyPlaylistFormat(PlaylistFormat format)
        {
            A.CallTo(() => MockControlFile.GetPlaylistFormat()).Returns(format);
            return this;
        }

        public ControlFileMocker ApplyRetryWaitInSeconds(int wait)
        {
            A.CallTo(() => MockControlFile.GetRetryWaitInSeconds()).Returns(wait);
            return this;
        }

        public ControlFileMocker ApplyDiagnosticRetainTemporaryFiles(bool flag)
        {
            A.CallTo(() => MockControlFile.GetDiagnosticRetainTemporaryFiles()).Returns(flag);
            return this;
        }

        public ControlFileMocker ApplyMaximumNumberOfConcurrentDownloads(int max)
        {
            A.CallTo(() => MockControlFile.GetMaximumNumberOfConcurrentDownloads()).Returns(max);
            return this;
        }

        public ControlFileMocker ApplyPodcasts(IEnumerable<IPodcastInfo> podcasts)
        {
            A.CallTo(() => MockControlFile.GetPodcasts()).Returns(podcasts);
            return this;
        }

    }

}