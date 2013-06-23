using System;
using System.IO;
using NUnit.Framework;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.Platform.Mtp.MtpDriveInfoProviderTests
{
    public class WhenGettingDriveInfoAndDeviceIsNotFound : WhenTestingMtpDriveInfoProvider
    {
        protected Exception ThrownException { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();

            DeviceManager.Stub(manager => manager.GetDevice("test device"))
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