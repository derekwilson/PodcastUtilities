using FakeItEasy;
using PodcastUtilities.Common.Platform;
using System.Collections.Generic;

namespace PodcastUtilities.AndroidTests.Helpers
{
    public class DirectoryInfoMocker
    {
        private IDirectoryInfo MockDir = A.Fake<IDirectoryInfo>();

        public IDirectoryInfo GetMockedDirectoryInfo() => MockDir;

        public List<IDirectoryInfo> GetMockedDirectoryInfoAsList() => new List<IDirectoryInfo>() { MockDir };

        public DirectoryInfoMocker ApplyFullName(string fullName)
        {
            A.CallTo(() => MockDir.FullName).Returns(fullName);
            return this;
        }
    }

}