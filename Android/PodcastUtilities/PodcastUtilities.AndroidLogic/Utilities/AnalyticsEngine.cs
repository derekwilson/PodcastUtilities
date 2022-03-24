using Android.Content;
using Android.OS;
using Firebase.Analytics;

namespace PodcastUtilities.AndroidLogic.Utilities
{
    public interface IAnalyticsEngine
    {
        void DownloadFeedEvent(int numberOfItems);
        void DownloadEpisodeEvent(long sizeInMB);
        void DownloadEpisodeCompleteEvent();
    }

    public class FirebaseAnalyticsEngine : IAnalyticsEngine
    {
        private Context ApplicationContext;

        public FirebaseAnalyticsEngine(Context applicationContext)
        {
            ApplicationContext = applicationContext;
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
    }
}