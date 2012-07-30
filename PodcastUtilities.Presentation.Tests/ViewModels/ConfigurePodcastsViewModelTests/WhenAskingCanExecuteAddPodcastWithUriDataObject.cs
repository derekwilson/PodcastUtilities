using System.Windows;
using NUnit.Framework;
using Rhino.Mocks;

namespace PodcastUtilities.Presentation.Tests.ViewModels.ConfigurePodcastsViewModelTests
{
    public class WhenAskingCanExecuteAddPodcastWithUriDataObject : WhenAskingCanExecuteAddPodcastCommand
    {
        protected override void GivenThat()
        {
            base.GivenThat();

            var dataObject = GenerateMock<IDataObject>();
            DataObjectUriExtractor.Stub(extractor => extractor.ContainsUri(dataObject))
                .Return(true);

            CommandParameter = dataObject;
        }

        [Test]
        public void ItShouldBeAbleToExecuteAddCommand()
        {
            Assert.That(CanExecuteAdd);
        }
    }
}