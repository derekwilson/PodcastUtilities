using System.Windows;
using NUnit.Framework;
using Rhino.Mocks;

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

    public class WhenAddingPodcastWithUrlTextDataObject : WhenAddingPodcastWithDataObject
    {
        protected override void GivenThat()
        {
            base.GivenThat();

            DataObject.Stub(data => data.GetData("Text"))
                .Return("http://www.test.com/podcast.xml");
        }

        [Test]
        public void ItShouldCreatePodcastWithAddressFromTheDataObject()
        {
            Assert.That(CreatedPodcast.Feed.Address, Is.Not.Null);
            Assert.That(CreatedPodcast.Feed.Address.AbsoluteUri, Is.EqualTo("http://www.test.com/podcast.xml"));
        }
    }

    public class WhenAddingPodcastWithNonUrlTextDataObject : WhenAddingPodcastWithDataObject
    {
        protected override void GivenThat()
        {
            base.GivenThat();

            DataObject.Stub(data => data.GetData("Text"))
                .Return("abcd");
        }


        [Test]
        public void ItShouldNotTryToUseTheDataObjectAsThePodcastAddress()
        {
            Assert.That(CreatedPodcast.Feed.Address, Is.Null);
        }
    }
}