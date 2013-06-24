using NUnit.Framework;
using PodcastUtilities.Common.Platform;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.Platform.FileSystemAwareFileUtilitiesTests.FileCopy
{
    public class WhenSourceFileIsMtp
        : WhenTestingFileUtilities
    {
        protected override void GivenThat()
        {
            base.GivenThat();

            DeviceManager.Stub(manager => manager.GetDevice("my device"))
                .Return(Device);

            Device.Stub(device => device.OpenRead(@"foo\bar.abc"))
                .Return(SourceStream);

            var fileInfo = GenerateMock<IFileInfo>();
            fileInfo.Stub(info => info.Length)
                .Return(1234);
            FileInfoProvider.Stub(provider => provider.GetFileInfo(@"MTP:\my device\foo\bar.abc"))
                .Return(fileInfo);

            StreamHelper.Stub(helper => helper.OpenWrite(@"D:\foo2\bar.abc", true))
                .Return(DestinationStream);
        }

        protected override void When()
        {
            Utilities.FileCopy(@"MTP:\my device\foo\bar.abc", @"D:\foo2\bar.abc", true);
        }

        [Test]
        public void ItShouldNotDelegateToRegularFileUtilities()
        {
            RegularFileUtilities.AssertWasNotCalled(
                utilities => utilities.FileCopy(null, null, true),
                options => options.IgnoreArguments());
        }

        [Test]
        public void ItShouldCopyFromSourceDeviceStreamToDestinationFileStream()
        {
            StreamHelper.AssertWasCalled(helper => helper.Copy(SourceStream, DestinationStream));
        }
    }
}