using Moq;
using NUnit.Framework;
using PodcastUtilities.Common.Platform;
using System;
using System.Collections.Generic;
using System.Text;

namespace PodcastUtilities.Common.Multiplatform.Tests.StateProviderTests
{
    public class WhenTestingTheStateProvider : WhenTestingBehaviour
    {
        protected StateProvider provider;
        protected IState state;
        protected Mock<IFileUtilities> fileUtilities;

        protected override void GivenThat()
        {
            base.GivenThat();
            fileUtilities = GenerateMock<IFileUtilities>();

            fileUtilities.Setup(utils => utils.FileExists(@"c:\folder\state.xml")).Returns(false);
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
