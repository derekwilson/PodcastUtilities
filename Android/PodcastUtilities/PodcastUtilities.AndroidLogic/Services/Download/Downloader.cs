using PodcastUtilities.AndroidLogic.Logging;
using PodcastUtilities.AndroidLogic.ViewModel.Download;
using PodcastUtilities.Common.Platform;
using System;
using System.Collections.Generic;
using static Android.InputMethodServices.Keyboard;

namespace PodcastUtilities.AndroidLogic.Services.Download
{
	public interface IDownloader
	{
        bool IsDownloading { get; }
        string NotifcationTitle { get; }
        string NotifcationText { get; }
        string NotifcationAccessibilityText { get; }

        void CancelAll();
        void SetDownloadItems(List<DownloadRecyclerItem> allItems);
    }

    public class Downloader : IDownloader
    {
        private ILogger Logger;
        private ITimeProvider TimeProvider;

        private bool DownloadingInProgress = false;

        public Downloader(ILogger logger, ITimeProvider timeProvider)
        {
            Logger = logger;
            TimeProvider = timeProvider;
        }

        public bool IsDownloading
        {
            get
            {
                return DownloadingInProgress;
            }
        }

        public string NotifcationTitle
        {
            get
            {
                return "Downloading";
            }
        }

        public string NotifcationText
        {
            get
            {
                return "Text";
            }
        }

        public string NotifcationAccessibilityText
        {
            get
            {
                return "Text";
            }
        }

        public void CancelAll()
        {
            Logger.Debug(() => $"Downloader:CancelAll");
            DownloadingInProgress = false;
        }

        public void SetDownloadItems(List<DownloadRecyclerItem> allItems)
        {
            Logger.Debug(() => $"Downloader:SetDownloadItems = {allItems?.Count}");
            DownloadingInProgress = true;
        }
    }
}