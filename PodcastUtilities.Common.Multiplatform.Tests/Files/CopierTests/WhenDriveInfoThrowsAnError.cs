using Moq;
using NUnit.Framework;

namespace PodcastUtilities.Common.Multiplatform.Tests.Files.CopierTests
{
    public class WhenDriveInfoThrowsAnError
        : WhenTestingCopier
    {
        protected override void GivenThat()
        {
            base.GivenThat();

            DestinationePath = @"e:\error";
            FileUtilities.Setup(u => u.FileExists(@"e:\error\A"))
                .Returns(false);
            FileUtilities.Setup(u => u.FileExists(@"e:\error\B"))
                .Returns(false);
        }

        [Test]
        public void ItShouldReportErrorStatusUpdateOnlyOnce()
        {
            Assert.AreEqual(3, StatusUpdates.Count);        // one for each file and only one for the error

            Assert.AreEqual(StatusUpdateLevel.Warning, StatusUpdates[1].MessageLevel);
            Assert.IsTrue(StatusUpdates[1].Message.Contains("Object must be a root directory"));
        }

        [Test]
        public void ItShouldTryToCopyAllTheFiles()
        {
            FileUtilities.Verify(utils => utils.FileCopy(@"c:\Source\A", @"e:\error\A"), Times.Once());
            FileUtilities.Verify(utils => utils.FileCopy(@"c:\Source\B", @"e:\error\B"), Times.Once());
        }
    }
}