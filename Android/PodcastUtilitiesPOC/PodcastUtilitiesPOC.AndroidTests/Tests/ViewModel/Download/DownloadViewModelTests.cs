using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using FakeItEasy;
using Moq;
using PodcastUtilities.Common;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Common.Feeds;
using PodcastUtilitiesPOC.AndroidLogic.Converter;
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
        protected IEpisodeFinder MockFinder = A.Fake<IEpisodeFinder>();
        protected ISyncItemToEpisodeDownloaderTaskConverter MockConverter = A.Fake<ISyncItemToEpisodeDownloaderTaskConverter>();
        protected ITaskPool MockTaskPool = A.Fake<ITaskPool>();
        protected IFileSystemHelper MockFilesystemHelper = A.Fake<IFileSystemHelper>();
        protected IByteConverter ByteConverter = new ByteConverter();

        protected ReadOnlyControlFile MockControlFile = A.Fake<ReadOnlyControlFile>();

        public DownloadViewModelTests()
        {
            A.CallTo(() => MockResourceProvider.GetString(Resource.String.download_activity_title)).Returns("Observed Mocked Title");
            ViewModel = new DownloadViewModel(MockApplication, MockLogger, MockResourceProvider, MockFinder, MockConverter, MockTaskPool, MockFilesystemHelper, ByteConverter);
            ViewModel.Observables.Title += SetTitle;
        }

        private void SetTitle(object sender, string title)
        {
            LastSetTitle = title;
        }

        [Fact]
        public void Initialise_Sets_Title()
        {
            ViewModel.Initialise(MockControlFile);

            Assert.Equal("Observed Mocked Title", LastSetTitle);
        }

        [Fact]
        public void Initialise_Loggs()
        {
            ViewModel.Initialise(MockControlFile);

            A.CallTo(() => MockLogger.Debug(A<ILogger.MessageGenerator>.Ignored)).MustHaveHappened(2, Times.Exactly);
        }
    }
}