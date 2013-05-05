using System;
using System.IO;
using NUnit.Framework;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.Platform.FileSystemAwareFileUtilitiesTests.FileDelete
{
    public class WhenPathIsMtpAndDeviceDoesNotExist
        : WhenTestingFileUtilities
    {
        public Exception ThrownException { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();

            DeviceManager.Stub(manager => manager.GetDevice("my device"))
                .Return(null);
        }

        protected override void When()
        {
            try
            {
                Utilities.FileDelete(@"mtp:\my device\foo\bar.abc");
            }
            catch (Exception exception)
            {
                ThrownException = exception;
            }
        }

        [Test]
        public void ItShouldThrowDirectoryNotFound()
        {
            Assert.That(ThrownException, Is.Not.Null);
            Assert.That(ThrownException, Is.InstanceOf<DirectoryNotFoundException>());
        }
    }
}