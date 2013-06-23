using System;
using System.IO;
using NUnit.Framework;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.Platform.Mtp.MtpDriveInfoProviderTests
{
    public class WhenGettingDriveInfoAndDeviceIsFoundButNoStorageObject : WhenGettingDriveInfoAndDeviceIsFound
    {
        protected Exception ThrownException { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();

            Device.Stub(device => device.GetRootStorageObjectFromPath(@"a\b\c"))
                .Return(null);
        }

        protected override void When()
        {
            try
            {
                DriveInfoProvider.GetDriveInfoForPath(@"mtp:\test device\a\b\c");
            }
            catch (Exception exception)
            {
                ThrownException = exception;
            }
        }

        [Test]
        public void ItShouldThrowDriveNotFoundException()
        {
            Assert.That(ThrownException, Is.Not.Null);
            Assert.That(ThrownException, Is.InstanceOf<DriveNotFoundException>());
        }
    }
}