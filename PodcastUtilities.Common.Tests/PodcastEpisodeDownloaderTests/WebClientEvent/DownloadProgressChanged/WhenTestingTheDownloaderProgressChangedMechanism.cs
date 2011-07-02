using System;
using System.ComponentModel;
using System.Net;
using System.Reflection;

namespace PodcastUtilities.Common.Tests.PodcastEpisodeDownloaderTests.WebClientEvent.DownloadProgressChanged
{
    public abstract class WhenTestingTheDownloaderProgressChangedMechanism : WhenTestingTheDownloader
    {
        protected DownloadProgressChangedEventArgs _progressEventArgs;
        protected long _bytesReceived;
        protected long _totalBytes;
        protected int _percentage;

        protected override void GivenThat()
        {
            base.GivenThat();

            _downloader.SyncItem = _syncItem;
            _downloader.Start(null);

            CreateData();

            _progressEventArgs = GetProgressEventArgs();
        }

        protected virtual void CreateData()
        {
        }

        private DownloadProgressChangedEventArgs GetProgressEventArgs()
        {
            DownloadProgressChangedEventArgs mockEventArgs =
             (DownloadProgressChangedEventArgs)System.Runtime.Serialization.FormatterServices
              .GetUninitializedObject(typeof(DownloadProgressChangedEventArgs));

            FieldInfo[] eventFields = typeof(DownloadProgressChangedEventArgs)
                        .GetFields(
                            BindingFlags.NonPublic |
                            BindingFlags.Instance |
                            BindingFlags.Public
                            );
     
            foreach (var eventField in eventFields)
            {
                if (eventField.Name == "m_BytesReceived")
                {
                    eventField.SetValue(mockEventArgs, _bytesReceived);
                }
                if (eventField.Name == "m_TotalBytesToReceive")
                {
                    eventField.SetValue(mockEventArgs, _totalBytes);
                }
            }

            FieldInfo[] baseEventFields = typeof(ProgressChangedEventArgs)
            .GetFields(
                BindingFlags.NonPublic |
                BindingFlags.Instance |
                BindingFlags.Public
                );

            foreach (var eventField in baseEventFields)
            {
                if (eventField.Name == "progressPercentage")
                {
                    eventField.SetValue(mockEventArgs, _percentage);
                }
                if (eventField.Name == "userState")
                {
                    eventField.SetValue(mockEventArgs, _syncItem);
                }
            }

            return mockEventArgs;
        }
    }
}