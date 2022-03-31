using Android.Content;
using Android.OS;
using Firebase.Analytics;
using PodcastUtilities.Common.Playlists;

namespace PodcastUtilities.AndroidLogic.Utilities
{
    public interface IAnalyticsEngine
    {
        void DownloadFeedEvent(int numberOfItems);
        void DownloadEpisodeEvent(long sizeInMB);
        void DownloadEpisodeCompleteEvent();
        void LoadControlFileEvent();
        void LifecycleLaunchEvent();
        void LifecycleErrorEvent();
        void LifecycleErrorFatalEvent();
        void GeneratePlaylistEvent(PlaylistFormat format);
    }

    public class FirebaseAnalyticsEngine : IAnalyticsEngine
    {
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
    }
}