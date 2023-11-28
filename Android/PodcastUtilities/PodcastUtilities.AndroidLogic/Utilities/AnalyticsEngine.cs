using Android.Content;
using Android.OS;
using Microsoft.AppCenter.Analytics;
using PodcastUtilities.Common.Playlists;
using System.Collections.Generic;

namespace PodcastUtilities.AndroidLogic.Utilities
{
    public interface IAnalyticsEngine
    {
        void DownloadFeedEvent(int numberOfItems);
        void DownloadSpecificFeedEvent(int numberOfItems, string folder);
        void DownloadEpisodeEvent(long sizeInMB);
        void DownloadEpisodeCompleteEvent();
        void LoadControlFileEvent();
        void ResetControlFileEvent();
        void ShareControlFileEvent();
        void LifecycleLaunchEvent(float scaling, string uiMode);
        void LifecycleErrorEvent();
        void LifecycleErrorFatalEvent();
        void GeneratePlaylistEvent(PlaylistFormat format);
        void GeneratePlaylistCompleteEvent(int numberOfItems);
        void PurgeScanEvent(int numberOfItems);
        void PurgeDeleteEvent(int numberOfItems);
        void ViewLogsEvent(int numberOfLines);
    }

    public class AppCenterAnalyticsEngine : IAnalyticsEngine
    {
        // to see the analytics go to
        // https://appcenter.ms/orgs/AndrewAndDerek/apps/PodcastUtilitiesDebug/analytics/overview

        private IAndroidApplication Application;
        private IAndroidEnvironmentInformationProvider AndroidEnvironmentInformationProvider;

        public AppCenterAnalyticsEngine(IAndroidApplication application, IAndroidEnvironmentInformationProvider androidEnvironmentInformationProvider)
        {
            Application = application;
            AndroidEnvironmentInformationProvider = androidEnvironmentInformationProvider;
        }

        private const string Event_Download_Feed = "Download_Feed";
        private const string Event_Download_Specific_Feed = "Download_Specific_Feed";
        private const string Event_Download_Episode = "Download_Episode";
        private const string Event_Download_Episode_Complete = "Download_Episode_Complete";
        private const string Event_Load_ControlFile = "Load_ControlFile";
        private const string Event_Reset_ControlFile = "Reset_ControlFile";
        private const string Event_Share_ControlFile = "Share_ControlFile";
        private const string Event_Lifecycle_Launch = "Lifecycle_Launch";
        private const string Event_Lifecycle_Error = "Lifecycle_Error";
        private const string Event_Lifecycle_ErrorFatal = "Lifecycle_ErrorFatal";
        private const string Event_Generate_Playlist = "Generate_Playlist";
        private const string Event_Generate_Playlist_Complete = "Generate_Playlist_Complete";
        private const string Event_Purge_Scan = "Purge_Scan";
        private const string Event_Purge_Delete = "Purge_Delete";
        private const string Event_View_Logs = "Logs_View";

        private const string Property_Version = "Version";
        private const string Property_FontScaling = "FontScaling";
        private const string Property_UIMode = "UIMode";
        private const string Property_IsKindle = "IsKindle";
        private const string Property_IsWsa = "IsWsa";
        private const string Property_SizeMB = "SizeMB";
        private const string Property_NumberOfItems = "NumberOfItems";
        private const string Property_Folder = "Folder";
        private const string Property_Format = "Format";

        public void DownloadEpisodeCompleteEvent()
        {
            Analytics.TrackEvent(Event_Download_Episode_Complete);
        }

        public void DownloadEpisodeEvent(long sizeInMB)
        {
            Analytics.TrackEvent(Event_Download_Episode, new Dictionary<string, string> {
                { Property_SizeMB, sizeInMB.ToString()}
            });
        }

        public void DownloadSpecificFeedEvent(int numberOfItems, string folder)
        {
            Analytics.TrackEvent(Event_Download_Specific_Feed, new Dictionary<string, string> {
                { Property_NumberOfItems, numberOfItems.ToString()},
                { Property_Folder, folder}
            });
        }

        public void DownloadFeedEvent(int numberOfItems)
        {
            Analytics.TrackEvent(Event_Download_Feed, new Dictionary<string, string> {
                { Property_NumberOfItems, numberOfItems.ToString()}
            });
        }

        public void GeneratePlaylistEvent(PlaylistFormat format)
        {
            Analytics.TrackEvent(Event_Generate_Playlist, new Dictionary<string, string> {
                { Property_Format, format.ToString()},
            });
        }
        public void GeneratePlaylistCompleteEvent(int numberOfItems)
        {
            Analytics.TrackEvent(Event_Generate_Playlist_Complete, new Dictionary<string, string> {
                { Property_NumberOfItems, numberOfItems.ToString()}
            });
        }

        public void LifecycleErrorEvent()
        {
            Analytics.TrackEvent(Event_Lifecycle_Error, new Dictionary<string, string> {
                { Property_Version, Application.DisplayVersion}
            });
        }

        public void LifecycleErrorFatalEvent()
        {
            Analytics.TrackEvent(Event_Lifecycle_ErrorFatal, new Dictionary<string, string> {
                { Property_Version, Application.DisplayVersion}
            });
        }

        public void LifecycleLaunchEvent(float scaling, string uiMode)
        {
            Analytics.TrackEvent(Event_Lifecycle_Launch, new Dictionary<string, string> {
                { Property_Version, Application.DisplayVersion},
                { Property_FontScaling, scaling.ToString()},
                { Property_UIMode, uiMode.ToString()},
                { Property_IsKindle, AndroidEnvironmentInformationProvider.IsKindleFire().ToString()},
                { Property_IsWsa, AndroidEnvironmentInformationProvider.IsWsa().ToString()},
            });
        }

        public void LoadControlFileEvent()
        {
            Analytics.TrackEvent(Event_Load_ControlFile);
        }

        public void ResetControlFileEvent()
        {
            Analytics.TrackEvent(Event_Reset_ControlFile);
        }

        public void ShareControlFileEvent()
        {
            Analytics.TrackEvent(Event_Share_ControlFile);
        }

        public void PurgeDeleteEvent(int numberOfItems)
        {
            Analytics.TrackEvent(Event_Purge_Delete, new Dictionary<string, string> {
                { Property_NumberOfItems, numberOfItems.ToString()}
            });
        }

        public void PurgeScanEvent(int numberOfItems)
        {
            Analytics.TrackEvent(Event_Purge_Scan, new Dictionary<string, string> {
                { Property_NumberOfItems, numberOfItems.ToString()}
            });
        }

        public void ViewLogsEvent(int numberOfLines)
        {
            Analytics.TrackEvent(Event_View_Logs, new Dictionary<string, string> {
                { Property_NumberOfItems, numberOfLines.ToString()}
            });
        }
    }

    /* 
     * - cannot leave this code here as we have removed the NuGet package references
     *

    public class FirebaseAnalyticsEngine : IAnalyticsEngine
    {
        // to see the analytics go to
        // https://console.firebase.google.com/project/podcastutilities/analytics/app/android:com.andrewandderek.podcastutilities.sideload.debug/overview

        private Context ApplicationContext;
        private IAndroidApplication Application;

        public FirebaseAnalyticsEngine(Context applicationContext, IAndroidApplication application)
        {
            ApplicationContext = applicationContext;
            Application = application;
        }

        private void SendEvent(string eventName, string eventCategory, string eventAction, string eventLabel, string eventValue)
        {
            var firebaseAnalytics = FirebaseAnalytics.GetInstance(ApplicationContext);
            var bundle = new Bundle();
            bundle.PutString(FirebaseAnalytics.Param.ItemId, eventCategory);
            if (eventAction != null)
            {
                bundle.PutString(FirebaseAnalytics.Param.ItemName, eventAction);
            }
            if (eventLabel != null)
            {
                bundle.PutString(FirebaseAnalytics.Param.ContentType, eventLabel);
            }
            if (eventValue != null)
            {
                bundle.PutString(FirebaseAnalytics.Param.Value, eventValue);
            }
            firebaseAnalytics.LogEvent(eventName, bundle);
        }

        private const string Seperator = "_";
        private const string Category_Download = "Download";
        private const string Action_Download_Feed = "Download_Feed";
        private const string Action_Download_Episode = "Download_Episode";
        private const string Action_Download_Episode_Complete = "Download_Episode_Complete";
        private const string Category_Load = "Load";
        private const string Action_Load_ControlFile = "Load_ControlFile";
        private const string Category_Lifecycle = "Lifecycle";
        private const string Action_Lifecycle_Launch = "Lifecycle_Launch";
        private const string Action_Lifecycle_Error = "Lifecycle_Error";
        private const string Action_Lifecycle_ErrorFatal = "Lifecycle_ErrorFatal";
        private const string Category_Generate = "Generate";
        private const string Action_Generate_Playlist = "Generate_Playlist";
        private const string Category_Purge = "Purge";
        private const string Action_Purge_Scan = "Purge_Scan";
        private const string Action_Purge_Delete = "Purge_Delete";

        public void DownloadFeedEvent(int numberOfItems)
        {
            SendEvent(
                FirebaseAnalytics.Event.SelectContent,
                Category_Download,
                Action_Download_Feed,
                Action_Download_Feed + Seperator + numberOfItems.ToString(),
                numberOfItems.ToString()
                );
        }

        public void DownloadEpisodeEvent(long sizeInMB)
        {
            SendEvent(
                FirebaseAnalytics.Event.SelectContent,
                Category_Download,
                Action_Download_Episode,
                Action_Download_Episode + Seperator + sizeInMB.ToString(),
                sizeInMB.ToString()
                );
        }

        public void DownloadEpisodeCompleteEvent()
        {
            SendEvent(
                FirebaseAnalytics.Event.SelectContent,
                Category_Download,
                Action_Download_Episode_Complete,
                null,
                null
                );
        }

        public void LoadControlFileEvent()
        {
            SendEvent(
                FirebaseAnalytics.Event.SelectContent,
                Category_Load,
                Action_Load_ControlFile,
                null,
                null
                );
        }

        public void LifecycleLaunchEvent()
        {
            SendEvent(
                FirebaseAnalytics.Event.SelectContent,
                Category_Lifecycle,
                Action_Lifecycle_Launch,
                Action_Lifecycle_Launch + Seperator + Application.DisplayVersion,
                null
                );
        }

        public void LifecycleErrorEvent()
        {
            SendEvent(
                FirebaseAnalytics.Event.SelectContent,
                Category_Lifecycle,
                Action_Lifecycle_Error,
                Action_Lifecycle_Error + Seperator + Application.DisplayVersion,
                null
                );
        }

        public void LifecycleErrorFatalEvent()
        {
            SendEvent(
                FirebaseAnalytics.Event.SelectContent,
                Category_Lifecycle,
                Action_Lifecycle_ErrorFatal,
                Action_Lifecycle_ErrorFatal + Seperator + Application.DisplayVersion,
                null
                );
        }

        public void GeneratePlaylistEvent(PlaylistFormat format)
        {
            SendEvent(
                FirebaseAnalytics.Event.SelectContent,
                Category_Generate,
                Action_Generate_Playlist,
                Action_Generate_Playlist + Seperator + format.ToString(),
                null
                );
        }

        public void PurgeScanEvent(int numberOfItems)
        {
            SendEvent(
                FirebaseAnalytics.Event.SelectContent,
                Category_Purge,
                Action_Purge_Scan,
                Action_Purge_Scan + Seperator + numberOfItems.ToString(),
                numberOfItems.ToString()
                );
        }

        public void PurgeDeleteEvent(int numberOfItems)
        {
            SendEvent(
                FirebaseAnalytics.Event.SelectContent,
                Category_Purge,
                Action_Purge_Delete,
                Action_Purge_Delete + Seperator + numberOfItems.ToString(),
                numberOfItems.ToString()
                );
        }
    }

    */
}