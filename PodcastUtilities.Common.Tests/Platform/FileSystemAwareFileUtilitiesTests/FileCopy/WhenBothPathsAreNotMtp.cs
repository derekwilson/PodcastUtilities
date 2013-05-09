using NUnit.Framework;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.Platform.FileSystemAwareFileUtilitiesTests.FileCopy
{
    public class WhenBothPathsAreNotMtp
        : WhenTestingFileUtilities
    {
        protected override void When()
        {
            Utilities.FileCopy(@"C:\foo\bar.abc", @"D:\foo2\bar.abc", true);
        }

        [Test]
        public void ItShouldDelegateToRegularFileUtilities()
        {
            RegularFileUtilities.AssertWasCalled(utilities => utilities.FileCopy(@"C:\foo\bar.abc", @"D:\foo2\bar.abc", true));
        }
    }
}