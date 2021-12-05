using Android.App;
using AndroidX.Lifecycle;
using FakeItEasy;
using PodcastUtilitiesPOC.AndroidLogic.Logging;
using PodcastUtilitiesPOC.AndroidLogic.Utilities;
using PodcastUtilitiesPOC.AndroidLogic.ViewModel.Example;
using Xunit;

namespace PodcastUtilitiesPOC.AndroidTests.Tests.ViewModel.Example
{
    public class ExampleViewModelTests
    {
        private ExampleViewModel ViewModel;
        private string LastSetTitle;
        private string LastSetBody;

        protected Application MockApplication = A.Fake<Application>();
        protected ILogger MockLogger = A.Fake<ILogger>();
        protected IResourceProvider MockResourceProvider = A.Fake<IResourceProvider>();
        protected ILifecycleOwner MockOwner = A.Fake<ILifecycleOwner>();

        public ExampleViewModelTests()
        {
            A.CallTo(() => MockResourceProvider.GetString(Resource.String.example_activity_body_text)).Returns("Body Text");
            ViewModel = new ExampleViewModel(MockApplication, MockLogger, MockResourceProvider);
            ViewModel.Observables.Title += SetTitle;
            ViewModel.Observables.Body += SetBody;
        }

        private void SetBody(object sender, string body)
        {
            LastSetBody = body;
        }

        private void SetTitle(object sender, string title)
        {
            LastSetTitle = title;
        }

        [Fact]
        public void Initialise_Sets_Title()
        {
            ViewModel.Initialise();

            Assert.Equal("Example Observed Title", LastSetTitle);
        }

        [Fact]
        public void Initialise_Sets_Body()
        {
            ViewModel.Initialise();

            Assert.Equal("Body Text", LastSetBody);
        }

        [Fact]
        public void Initialise_Loggs()
        {
            ViewModel.Initialise();

            A.CallTo(() => MockLogger.Debug(A<ILogger.MessageGenerator>.Ignored)).MustHaveHappened(2, Times.Exactly);
        }
    }
}