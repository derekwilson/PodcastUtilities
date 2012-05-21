using System.Windows;
using NUnit.Framework;
using Rhino.Mocks;

namespace PodcastUtilities.Presentation.Tests.ViewModels.ConfigurePodcastsViewModelTests
{
    public class WhenAskingCanExecuteAddPodcastWithNonUriDataObject : WhenAskingCanExecuteAddPodcastCommand
    {
        protected override void GivenThat()
        {
            base.GivenThat();

            var dataObject = GenerateMock<IDataObject>();
            DataObjectUriExtractor.Stub(extractor => extractor.ContainsUri(dataObject))
                .Return(false);

            CommandParameter = dataObject;
        }

        [Test]
        public void ItShouldNotBeAbleToExecuteAddCommand()
        {
            Assert.That(CanExecuteAdd, Is.False);
        }
    }
}