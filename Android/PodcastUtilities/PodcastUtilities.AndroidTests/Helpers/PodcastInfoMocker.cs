using FakeItEasy;
using PodcastUtilities.Common.Configuration;
using System.Collections.Generic;

namespace PodcastUtilities.AndroidTests.Helpers
{
    internal class PodcastInfoMocker
    {
        private IPodcastInfo MockPodcastInfo = A.Fake<IPodcastInfo>();

        public IPodcastInfo GetMockedPodcastInfo() => MockPodcastInfo;
        public List<IPodcastInfo> GetMockedPodcastInfoAsList() => new List<IPodcastInfo>() { MockPodcastInfo };

        public PodcastInfoMocker ApplyFolder(string folder)
        {
            A.CallTo(() => MockPodcastInfo.Folder).Returns(folder);
            return this;
        }

    }

}