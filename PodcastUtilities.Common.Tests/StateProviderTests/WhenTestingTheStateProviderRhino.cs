using NUnit.Framework;
using PodcastUtilities.Common.Platform;
using Rhino.Mocks;

namespace PodcastUtilities.Common.Tests.StateProviderTests
{
    class WhenTestingTheStateProviderRhino
        : WhenTestingBehaviour
    {
        protected StateProvider _provider;
        protected IState _state;
        protected IFileUtilities _fileUtilities;

        protected override void GivenThat()
        {
            base.GivenThat();
            _fileUtilities = GenerateMock<IFileUtilities>();

            _fileUtilities.Stub(utils => utils.FileExists(@"c:\folder\state.xml")).Return(false);
            _provider = new StateProvider(_fileUtilities);
        }

        protected override void When()
        {
            _state = _provider.GetState(@"c:\folder");
        }

        [Test]
        public void ItShouldGetTheState()
        {
            Assert.IsInstanceOf(typeof(IState), _state);
        }
    }
}
