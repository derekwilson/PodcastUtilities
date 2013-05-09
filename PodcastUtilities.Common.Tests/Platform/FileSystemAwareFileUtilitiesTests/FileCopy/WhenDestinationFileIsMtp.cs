using NUnit.Framework;
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

            Device.Stub(device => device.OpenWrite(@"path\file.xyz", true))
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