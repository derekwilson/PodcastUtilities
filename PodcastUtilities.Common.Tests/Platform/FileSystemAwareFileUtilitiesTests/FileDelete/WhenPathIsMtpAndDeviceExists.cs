using NUnit.Framework;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.Platform.FileSystemAwareFileUtilitiesTests.FileDelete
{
    public class WhenPathIsMtpAndDeviceExists
        : WhenTestingFileUtilities
    {
        protected override void GivenThat()
        {
            base.GivenThat();

            DeviceManager.Stub(manager => manager.GetDevice("my device"))
                .Return(Device);
        }

        protected override void When()
        {
            Utilities.FileDelete(@"mtp:\my device\foo\bar.abc");
        }

        [Test]
        public void ItShouldDelegateToDevice()
        {
            Device.AssertWasCalled(device => device.Delete(@"foo\bar.abc"));
        }
    }
}