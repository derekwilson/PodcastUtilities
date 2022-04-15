using FakeItEasy;
using PodcastUtilities.Common.Configuration;
using System.Collections.Generic;

namespace PodcastUtilities.AndroidTests.Helpers
{
    internal class ControlFileMocker
    {
        private IReadWriteControlFile MockControlFile = A.Fake<IReadWriteControlFile>();

        public IReadWriteControlFile GetMockedControlFile() => MockControlFile;

        public ControlFileMocker ApplySourceRoot(string root)
        {
            A.CallTo(() => MockControlFile.GetSourceRoot()).Returns(root);
            return this;
        }

        public ControlFileMocker ApplyPodcasts(IEnumerable<IPodcastInfo> podcasts)
        {
            A.CallTo(() => MockControlFile.GetPodcasts()).Returns(podcasts);
            return this;
        }

    }

}