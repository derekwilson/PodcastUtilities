using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using FakeItEasy;
using Moq;
using PodcastUtilitiesPOC.AndroidLogic.Logging;
using PodcastUtilitiesPOC.AndroidLogic.Utilities;
using PodcastUtilitiesPOC.AndroidLogic.ViewModel.Download;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Xunit.Abstractions;
using Times = FakeItEasy.Times;

namespace PodcastUtilitiesPOC.AndroidTests.Tests.ViewModel.Download
{
    public class DownloadViewModelTests
    {
        private DownloadViewModel ViewModel;
        private string LastSetTitle;

        protected Application MockApplication = A.Fake<Application>();
        protected ILogger MockLogger = A.Fake<ILogger>();
        protected IResourceProvider MockResourceProvider = A.Fake<IResourceProvider>();

        public DownloadViewModelTests()
        {
            A.CallTo(() => MockResourceProvider.GetString(Resource.String.download_activity_title)).Returns("Observed Mocked Title");
            ViewModel = new DownloadViewModel(MockApplication, MockLogger, MockResourceProvider);
            ViewModel.Observables.Title += SetTitle;
        }

        private void SetTitle(object sender, string title)
        {
            LastSetTitle = title;
        }

        [Fact]
        public void Initialise_Sets_Title()
        {
            ViewModel.Initialise();

            Assert.Equal("Observed Mocked Title", LastSetTitle);
        }

        [Fact]
        public void Initialise_Loggs()
        {
            ViewModel.Initialise();

            A.CallTo(() => MockLogger.Debug(A<ILogger.MessageGenerator>.Ignored)).MustHaveHappened(2, Times.Exactly);
        }
    }
}