using FakeItEasy;
using PodcastUtilities.Common.Platform;
using System.Collections.Generic;

namespace PodcastUtilities.AndroidTests.Helpers
{
    public class FileInfoMocker
    {
        private IFileInfo MockFile = A.Fake<IFileInfo>();

        public IFileInfo GetMockedFileInfo() => MockFile;
        public List<IFileInfo> GetMockedFileInfoAsList() => new List<IFileInfo>() { MockFile };

        public FileInfoMocker ApplyName(string name)
        {
            A.CallTo(() => MockFile.Name).Returns(name);
            return this;
        }

        public FileInfoMocker ApplyFullName(string fullName)
        {
            A.CallTo(() => MockFile.FullName).Returns(fullName);
            return this;
        }
    }

}