using AndroidX.Lifecycle;
using FakeItEasy;
using PodcastUtilitiesPOC.AndroidLogic.Logging;
using PodcastUtilitiesPOC.AndroidLogic.Utilities;
using PodcastUtilitiesPOC.AndroidLogic.ViewModel;
using PodcastUtilitiesPOC.AndroidLogic.ViewModel.Example;
using System.Linq;
using Xunit;

namespace PodcastUtilitiesPOC.AndroidTests.Tests.ViewModel.Example
{
    public class ExampleViewModelTests
    {
        private ExampleViewModel ViewModel;
        private string LastSetTitle;
        private string LastSetBody;

        protected Android.App.Application MockApplication = A.Fake<Android.App.Application>();
        protected ILogger MockLogger = A.Fake<ILogger>();
        protected ILiveDataFactory MockLiveDataFactory = A.Fake<ILiveDataFactory>();
        protected IResourceProvider MockResourceProvider = A.Fake<IResourceProvider>();
        protected MutableLiveData MockTitleLiveData = A.Fake<MutableLiveData>();

        public ExampleViewModelTests()
        {
            A.CallTo(() => MockResourceProvider.GetString(Resource.String.example_activity_body_text)).Returns("Body Text");
            A.CallTo(() => MockLiveDataFactory.CreateMutableLiveData()).Returns(MockTitleLiveData);
            ViewModel = new ExampleViewModel(MockApplication, MockLogger, MockResourceProvider, MockLiveDataFactory);
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

            Assert.Equal("Observed Title", LastSetTitle);
        }

        [Fact]
        public void Initialise_Sets_LiveData_Title()
        {
            ViewModel.Initialise();

            var calls = Fake.GetCalls(MockTitleLiveData).ToList();
            Assert.True(1 == calls.Count, $"there should be one call to the livedata there was {calls.Count} ");
            A.CallTo(() => MockTitleLiveData.PostValue(A<Java.Lang.Object>.That.Matches(s => s.ToString() == "Observed LiveData Title"))).MustHaveHappened(1, Times.Exactly);
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