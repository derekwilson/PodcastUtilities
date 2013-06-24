using NUnit.Framework;
using PodcastUtilities.Common.Platform;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.Platform.FileSystemAwareFileUtilitiesTests.FileCopy
{
    public class WhenDestinationFileIsMtp
        : WhenTestingFileUtilities
    {
        protected override void GivenThat()
        {
            base.GivenThat();

            DeviceManager.Stub(manager => manager.GetDevice("test device"))
                .Return(Device);

            StreamHelper.Stub(helper => helper.OpenRead(@"D:\foo2\bar.abc"))
                .Return(SourceStream);

            var fileInfo = GenerateMock<IFileInfo>();
            fileInfo.Stub(info => info.Length)
                .Return(1234);
            FileInfoProvider.Stub(provider => provider.GetFileInfo(@"D:\foo2\bar.abc"))
                .Return(fileInfo);

            Device.Stub(device => device.OpenWrite(@"path\file.xyz", 1234, true))
                .Return(DestinationStream);
        }

        protected override void When()
        {
            Utilities.FileCopy(@"D:\foo2\bar.abc", @"MTP:\test device\path\file.xyz", true);
        }

        [Test]
        public void ItShouldNotDelegateToRegularFileUtilities()
        {
            RegularFileUtilities.AssertWasNotCalled(
                utilities => utilities.FileCopy(null, null, true),
                options => options.IgnoreArguments());
        }

        [Test]
        public void ItShouldCopyFromSourceFileStreamToDestinationDeviceStream()
        {
            StreamHelper.AssertWasCalled(helper => helper.Copy(SourceStream, DestinationStream));
        }
    }
}