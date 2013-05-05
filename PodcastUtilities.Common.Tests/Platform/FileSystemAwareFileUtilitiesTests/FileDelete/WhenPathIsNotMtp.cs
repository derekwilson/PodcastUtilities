using NUnit.Framework;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.Platform.FileSystemAwareFileUtilitiesTests.FileDelete
{
    public class WhenPathIsNotMtp
        : WhenTestingFileUtilities
    {
        protected override void When()
        {
            Utilities.FileDelete(@"C:\foo\bar.abc");
        }

        [Test]
        public void ItShouldDelegateToRegularFileUtilities()
        {
            RegularFileUtilities.AssertWasCalled(utilities => utilities.FileDelete(@"C:\foo\bar.abc"));
        }
    }
}