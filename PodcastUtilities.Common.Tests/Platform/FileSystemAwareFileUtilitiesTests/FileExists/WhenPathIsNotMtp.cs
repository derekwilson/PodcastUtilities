using NUnit.Framework;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.Platform.FileSystemAwareFileUtilitiesTests.FileExists
{
    public class WhenPathIsNotMtp
        : WhenTestingFileUtilities
    {
        protected override void GivenThat()
        {
            base.GivenThat();

            RegularFileUtilities.Stub(utilities => utilities.FileExists(@"C:\foo\bar.abc"))
                .Return(true);
        }

        protected override void When()
        {
            Utilities.FileExists(@"C:\foo\bar.abc");
        }

        [Test]
        public void ItShouldDelegateToRegularFileUtilities()
        {
            RegularFileUtilities.AssertWasCalled(utilities => utilities.FileExists(@"C:\foo\bar.abc"));
        }
    }
}