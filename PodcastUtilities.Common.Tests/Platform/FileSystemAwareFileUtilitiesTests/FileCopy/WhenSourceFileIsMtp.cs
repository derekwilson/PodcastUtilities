using NUnit.Framework;
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