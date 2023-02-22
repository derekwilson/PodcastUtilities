using Moq;
using NUnit.Framework;
using PodcastUtilities.Common.Platform;

namespace PodcastUtilities.Common.Multiplatform.Tests.StateProviderTests
{
    public class WhenTestingTheStateProviderError : WhenTestingBehaviour
    {
        protected StateProvider provider;
        protected IState state;
        protected Mock<IFileUtilities> fileUtilities;

        protected override void GivenThat()
        {
            base.GivenThat();
            fileUtilities = GenerateMock<IFileUtilities>();

            fileUtilities.Setup(utils => utils.FileExists(@"c:\folder\state.xml")).Returns(true);
            provider = new StateProvider(fileUtilities.Object);
        }

        protected override void When()
        {
            state = provider.GetState(@"c:\folder");
        }

        [Test]
        public void ItShouldGetTheState()
        {
            Assert.IsInstanceOf(typeof(IState), state);
        }
    }

}
