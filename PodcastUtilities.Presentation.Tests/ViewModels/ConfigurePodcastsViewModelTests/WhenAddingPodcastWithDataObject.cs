using System.Windows;

namespace PodcastUtilities.Presentation.Tests.ViewModels.ConfigurePodcastsViewModelTests
{
    public abstract class WhenAddingPodcastWithDataObject : WhenExecutingAddPodcastCommand
    {
        protected IDataObject DataObject { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();

            DataObject = GenerateMock<IDataObject>();
        }

        protected override void When()
        {
            ViewModel.AddPodcastCommand.Execute(DataObject);
        }

    }
}