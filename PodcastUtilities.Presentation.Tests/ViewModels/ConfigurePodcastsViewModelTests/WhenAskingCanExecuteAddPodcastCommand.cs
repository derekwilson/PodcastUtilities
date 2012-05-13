using System.Windows;
using NUnit.Framework;
using Rhino.Mocks;

namespace PodcastUtilities.Presentation.Tests.ViewModels.ConfigurePodcastsViewModelTests
{
    public abstract class WhenAskingCanExecuteAddPodcastCommand
        : WhenTestingConfigurePodcastsViewModel
    {
        protected bool CanExecuteAdd { get; set; }

        protected object CommandParameter { get; set; }

        protected override void When()
        {
            CanExecuteAdd = ViewModel.AddPodcastCommand.CanExecute(CommandParameter);
        }
    }

    public class WhenAskingCanExecuteAddPodcastWithNull : WhenAskingCanExecuteAddPodcastCommand
    {
        protected override void GivenThat()
        {
            base.GivenThat();

            CommandParameter = null;
        }

        [Test]
        public void ItShouldBeAbleToExecuteAddCommand()
        {
            Assert.That(CanExecuteAdd);
        }
    }

    public class WhenAskingCanExecuteAddPodcastWithNonDataObject : WhenAskingCanExecuteAddPodcastCommand
    {
        protected override void GivenThat()
        {
            base.GivenThat();

            CommandParameter = "";
        }

        [Test]
        public void ItShouldBeAbleToExecuteAddCommand()
        {
            Assert.That(CanExecuteAdd);
        }
    }

    public class WhenAskingCanExecuteAddPodcastWithNonTextDataObject : WhenAskingCanExecuteAddPodcastCommand
    {
        protected override void GivenThat()
        {
            base.GivenThat();

            var dataObject = GenerateMock<IDataObject>();
            dataObject.Stub(data => data.GetData("Text"))
                .Return(null);

            CommandParameter = dataObject;
        }

        [Test]
        public void ItShouldNotBeAbleToExecuteAddCommand()
        {
            Assert.That(CanExecuteAdd, Is.False);
        }
    }

    public class WhenAskingCanExecuteAddPodcastWithNonUrlTextDataObject : WhenAskingCanExecuteAddPodcastCommand
    {
        protected override void GivenThat()
        {
            base.GivenThat();

            var dataObject = GenerateMock<IDataObject>();
            dataObject.Stub(data => data.GetData("Text"))
                .Return("fred");

            CommandParameter = dataObject;
        }

        [Test]
        public void ItShouldNotBeAbleToExecuteAddCommand()
        {
            Assert.That(CanExecuteAdd, Is.False);
        }
    }

    public class WhenAskingCanExecuteAddPodcastWithUrlTexDataObject : WhenAskingCanExecuteAddPodcastCommand
    {
        protected override void GivenThat()
        {
            base.GivenThat();

            var dataObject = GenerateMock<IDataObject>();
            dataObject.Stub(data => data.GetData("Text"))
                .Return("http://www.blah.com/xxx");

            CommandParameter = dataObject;
        }

        [Test]
        public void ItShouldBeAbleToExecuteAddCommand()
        {
            Assert.That(CanExecuteAdd);
        }
    }
}