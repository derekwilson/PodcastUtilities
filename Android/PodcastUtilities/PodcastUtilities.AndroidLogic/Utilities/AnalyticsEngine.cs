using Android.Content;
using Android.Content.Res;
using Mixpanel;
using PodcastUtilities.AndroidLogic.Logging;
using PodcastUtilities.Common.Playlists;
using System;
using System.Collections.Generic;

namespace PodcastUtilities.AndroidLogic.Utilities
{
    public interface IAnalyticsEngine
    {
        void DownloadFeedEvent(int numberOfItems);
        void DownloadSpecificFeedEvent(int numberOfItems, string folder);
        void TestSpecificFeedEvent(int numberOfItems, string folder);
        void DownloadEpisodeEvent(long sizeInMB);
        void DownloadEpisodeCompleteEvent();
        void LoadControlFileEvent(int numberOfItems);
        void ResetControlFileEvent();
        void ShareControlFileEvent(int numberOfItems);
        void LifecycleLaunchEvent();
        void LifecycleErrorEvent();
        void LifecycleErrorFatalEvent();
        void GeneratePlaylistEvent(PlaylistFormat format);
        void GeneratePlaylistCompleteEvent(int numberOfItems);
        void PurgeScanEvent(int numberOfItems);
        void PurgeDeleteEvent(int numberOfItems);
        void ViewPageEvent(string title, int numberOfItems);
        void AddPodcastEvent(string folder);
        void AddPodcastFeedEvent(string url);
        void RemovePodcastEvent(string folder);

        public const string Page_Logs = "Logs";
        public const string Page_Help = "Help";
        public const string Page_Osl = "Osl";
        public const string Page_Privacy = "Privacy";
    }

    /// <summary>
    /// an analytics engine that emits to Mixpanel
    /// https://eu.mixpanel.com/project/3318241/view/3822857/app/home
    /// </summary>
    public class MixpanelAnalyticsEngine : IAnalyticsEngine
    {
        private const string Event_Download_Feed = "Download_Feed";
        private const string Event_Download_Specific_Feed = "Download_Specific_Feed";
        private const string Event_Test_Specific_Feed = "Test_Specific_Feed";
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
        private const string Event_View_Page = "Page_View";
        private const string Event_Add_Podcast = "Config_Add_Podcast";
        private const string Event_Add_Podcast_Feed = "Config_Add_Podcast_Feed";
        private const string Event_Remove_Podcast = "Config_Remove_Podcast";

        private const string Mixpanel_Property_Manufacturer = "$manufacturer";
        private const string Mixpanel_Property_Brand = "$brand";
        private const string Mixpanel_Property_Model = "$model";
        private const string Mixpanel_Property_OsVersion = "$os_version";
        private const string Mixpanel_Property_AppVersion = "$app_version_string";

        private const string Property_FontScaling = "FontScaling";
        private const string Property_UIMode = "UIMode";
        private const string Property_IsKindle = "IsKindle";
        private const string Property_IsWsa = "IsWsa";
        private const string Property_SizeMB = "SizeMB";
        private const string Property_Name = "Name";
        private const string Property_NumberOfItems = "NumberOfItems";
        private const string Property_Folder = "Folder";
        private const string Property_Url = "Url";
        private const string Property_Format = "Format";

        private IAndroidApplication Application;
        private IAndroidEnvironmentInformationProvider AndroidEnvironmentInformationProvider;
        private ICrashReporter CrashReporter;
        private ILogger Logger;

        private MixpanelClient? MixpanelClient;

        public MixpanelAnalyticsEngine(
            IAndroidApplication application,
            IAndroidEnvironmentInformationProvider androidEnvironmentInformationProvider,
            ICrashReporter crashReporter,
            ILogger logger)
        {
            AndroidEnvironmentInformationProvider = androidEnvironmentInformationProvider;
            CrashReporter = crashReporter;
            Logger = logger;
            Application = application;

            Logger.Debug(() => $"MixpanelAnalyticsEngine - init - start {Secrets.MIXPANEL_PROJECT_TOKEN}");
            try
            {
                // log all errors
                MixpanelConfig.Global.ErrorLogFn = LogMixpanelErrors;
                // enable IP address geolocation
                MixpanelConfig.Global.IpAddressHandling = MixpanelIpAddressHandling.UseRequestIp;

                MixpanelClient = new MixpanelClient(
                    Secrets.MIXPANEL_PROJECT_TOKEN,
                    null,
                    new Dictionary<string, object>
                    {
                        {MixpanelProperty.DistinctId, $"$device:{androidEnvironmentInformationProvider.DeviceId}"},
                        {Mixpanel_Property_OsVersion, androidEnvironmentInformationProvider.OsVersion},
                        {Mixpanel_Property_Manufacturer, androidEnvironmentInformationProvider.Manufacturer},
                        {Mixpanel_Property_Brand, androidEnvironmentInformationProvider.Brand},
                        {Mixpanel_Property_Model, androidEnvironmentInformationProvider.Model},
                        {Mixpanel_Property_AppVersion, application.DisplayVersion},
                    }
                    );
                Logger.Debug(() => $"MixpanelAnalyticsEngine - init - done - {androidEnvironmentInformationProvider.DeviceId}");
            }
            catch (Exception ex)
            {
                // dont let the analytics crash the app
                Logger.LogException(() => $"MixpanelAnalyticsEngine - init", ex);
                crashReporter.LogNonFatalException(ex);
            };
            Logger.Debug(() => $"MixpanelAnalyticsEngine - init - end");
        }

        public void LogMixpanelErrors(string message, Exception exception)
        {
            Logger.LogException(() => $"MixpanelAnalyticsEngine - error - {message}", exception);
            CrashReporter.LogNonFatalException($"MixpanelAnalyticsEngine - error - {message}", exception);
        }

        private void trackEventAsync(string eventName, object? properties) {
            if (MixpanelClient == null)
            {
                Logger.Warning(() => $"MixpanelAnalyticsEngine - trackEvent - {eventName} - Mixpanel not init");
                return;
            }
            lock (MixpanelClient) {
                try
                {
                    MixpanelClient.TrackAsync(eventName, properties);
                    Logger.Debug(() => $"MixpanelAnalyticsEngine - trackEvent - {eventName}");
                } 
                catch (Exception ex) 
                {
                    // dont let the analytics crash the app
                    Logger.LogException(() => $"MixpanelAnalyticsEngine - trackEvent - {eventName}", ex);
                    CrashReporter.LogNonFatalException(ex);
                }
            }
        }

        public void DownloadEpisodeCompleteEvent()
        {
            trackEventAsync(Event_Download_Episode_Complete, null);
        }

        public void DownloadEpisodeEvent(long sizeInMB)
        {
            trackEventAsync(Event_Download_Episode,
                new Dictionary<string, object>
                {
                    {Property_SizeMB, sizeInMB}
                }
            );
        }

        public void DownloadSpecificFeedEvent(int numberOfItems, string folder)
        {
            trackEventAsync(Event_Download_Specific_Feed,
                new Dictionary<string, object>
                {
                    {Property_NumberOfItems, numberOfItems},
                    {Property_Folder, folder}
                }
            );
        }

        public void TestSpecificFeedEvent(int numberOfItems, string folder)
        {
            trackEventAsync(Event_Test_Specific_Feed,
                new Dictionary<string, object>
                {
                    {Property_NumberOfItems, numberOfItems},
                    {Property_Folder, folder}
                }
            );
        }

        public void DownloadFeedEvent(int numberOfItems)
        {
            trackEventAsync(Event_Download_Feed,
                new Dictionary<string, object>
                {
                    {Property_NumberOfItems, numberOfItems}
                }
            );
        }

        public void GeneratePlaylistEvent(PlaylistFormat format)
        {
            trackEventAsync(Event_Generate_Playlist,
                new Dictionary<string, object>
                {
                    {Property_Format, format.ToString()},
                }
            );
        }
        public void GeneratePlaylistCompleteEvent(int numberOfItems)
        {
            trackEventAsync(Event_Generate_Playlist_Complete,
                new Dictionary<string, object>
                {
                    {Property_NumberOfItems, numberOfItems}
                }
            );
        }

        public void LifecycleErrorEvent()
        {
            trackEventAsync(Event_Lifecycle_Error, null);
        }

        public void LifecycleErrorFatalEvent()
        {
            trackEventAsync(Event_Lifecycle_ErrorFatal, null);
        }

        public void LifecycleLaunchEvent()
        {
            trackEventAsync(Event_Lifecycle_Launch,
                new Dictionary<string, object>
                {
                    {Property_FontScaling, AndroidEnvironmentInformationProvider.FontScaling.ToString()},
                    {Property_UIMode, AndroidEnvironmentInformationProvider.UiMode},
                    {Property_IsKindle, AndroidEnvironmentInformationProvider.IsKindleFire().ToString()},
                    {Property_IsWsa, AndroidEnvironmentInformationProvider.IsWsa().ToString()},
                }
            );
        }

        public void LoadControlFileEvent(int numberOfItems)
        {
            trackEventAsync(Event_Load_ControlFile,
                new Dictionary<string, object>
                {
                    {Property_NumberOfItems, numberOfItems}
                }
            );
        }

        public void ResetControlFileEvent()
        {
            trackEventAsync(Event_Reset_ControlFile, null);
        }

        public void ShareControlFileEvent(int numberOfItems)
        {
            trackEventAsync(Event_Share_ControlFile,
                new Dictionary<string, object>
                {
                    {Property_NumberOfItems, numberOfItems}
                }
            );
        }

        public void PurgeDeleteEvent(int numberOfItems)
        {
            trackEventAsync(Event_Purge_Delete,
                new Dictionary<string, object>
                {
                    {Property_NumberOfItems, numberOfItems}
                }
            );
        }

        public void PurgeScanEvent(int numberOfItems)
        {
            trackEventAsync(Event_Purge_Scan,
                new Dictionary<string, object>
                {
                    {Property_NumberOfItems, numberOfItems}
                }
            );
        }

        public void ViewPageEvent(string title, int numberOfItems)
        {
            trackEventAsync(Event_View_Page,
                new Dictionary<string, object>
                {
                    {Property_Name, title},
                    {Property_NumberOfItems, numberOfItems}
                }
            );
        }

        public void AddPodcastEvent(string folder)
        {
            trackEventAsync(Event_Add_Podcast,
                new Dictionary<string, object>
                {
                    {Property_Folder, folder}
                }
            );
        }

        public void AddPodcastFeedEvent(string url)
        {
            trackEventAsync(Event_Add_Podcast_Feed,
                new Dictionary<string, object>
                {
                    {Property_Url, url}
                }
            );
        }

        public void RemovePodcastEvent(string folder)
        {
            trackEventAsync(Event_Remove_Podcast,
                new Dictionary<string, object>
                {
                    {Property_Folder, folder}
                }
            );
        }
    }

    /// <summary>
    /// a null analytics sink to discard all analytics - no dependencies
    /// </summary>
    public class NullAnalyticsEngine : IAnalyticsEngine
    {
        public void AddPodcastEvent(string folder)
        {
        }

        public void AddPodcastFeedEvent(string url)
        {
        }

        public void DownloadEpisodeCompleteEvent()
        {
        }

        public void DownloadEpisodeEvent(long sizeInMB)
        {
        }

        public void DownloadFeedEvent(int numberOfItems)
        {
        }

        public void DownloadSpecificFeedEvent(int numberOfItems, string folder)
        {
        }

        public void GeneratePlaylistCompleteEvent(int numberOfItems)
        {
        }

        public void GeneratePlaylistEvent(PlaylistFormat format)
        {
        }

        public void LifecycleErrorEvent()
        {
        }

        public void LifecycleErrorFatalEvent()
        {
        }

        public void LifecycleLaunchEvent()
        {
        }

        public void LoadControlFileEvent(int numberOfItems)
        {
        }

        public void PurgeDeleteEvent(int numberOfItems)
        {
        }

        public void PurgeScanEvent(int numberOfItems)
        {
        }

        public void RemovePodcastEvent(string folder)
        {
        }

        public void ResetControlFileEvent()
        {
        }

        public void ShareControlFileEvent(int numberOfItems)
        {
        }

        public void TestSpecificFeedEvent(int numberOfItems, string folder)
        {
        }

        public void ViewPageEvent(string title, int numberOfItems)
        {
        }
    }

    /* 
     * - cannot leave this code here as we have removed the NuGet package references
     *
    
    /// <summary>
    /// an analytics engine that emits to AppCenter
    /// https://appcenter.ms/orgs/AndrewAndDerek/apps/PodcastUtilitiesDebug/analytics/overview
    /// </summary>
    public class AppCenterAnalyticsEngine : IAnalyticsEngine
    {
        private IAndroidApplication Application;
        private IAndroidEnvironmentInformationProvider AndroidEnvironmentInformationProvider;

        public AppCenterAnalyticsEngine(IAndroidApplication application, IAndroidEnvironmentInformationProvider androidEnvironmentInformationProvider)
        {
            Application = application;
            AndroidEnvironmentInformationProvider = androidEnvironmentInformationProvider;
        }

        private const string Event_Download_Feed = "Download_Feed";
        private const string Event_Download_Specific_Feed = "Download_Specific_Feed";
        private const string Event_Test_Specific_Feed = "Test_Specific_Feed";
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
        private const string Event_Add_Podcast = "Config_Add_Podcast";
        private const string Event_Add_Podcast_Feed = "Config_Add_Podcast_Feed";
        private const string Event_Remove_Podcast = "Config_Remove_Podcast";

        private const string Property_Version = "Version";
        private const string Property_FontScaling = "FontScaling";
        private const string Property_UIMode = "UIMode";
        private const string Property_IsKindle = "IsKindle";
        private const string Property_IsWsa = "IsWsa";
        private const string Property_SizeMB = "SizeMB";
        private const string Property_NumberOfItems = "NumberOfItems";
        private const string Property_Folder = "Folder";
        private const string Property_Url = "Url";
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

        public void TestSpecificFeedEvent(int numberOfItems, string folder)
        {
            Analytics.TrackEvent(Event_Test_Specific_Feed, new Dictionary<string, string> {
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

        public void LoadControlFileEvent(int numberOfItems)
        {
            Analytics.TrackEvent(Event_Load_ControlFile, new Dictionary<string, string> {
                { Property_NumberOfItems, numberOfItems.ToString()}
            });
        }

        public void ResetControlFileEvent()
        {
            Analytics.TrackEvent(Event_Reset_ControlFile);
        }

        public void ShareControlFileEvent(int numberOfItems)
        {
            Analytics.TrackEvent(Event_Share_ControlFile, new Dictionary<string, string> {
                { Property_NumberOfItems, numberOfItems.ToString()}
            });
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

        public void AddPodcastEvent(string folder)
        {
            Analytics.TrackEvent(Event_Add_Podcast, new Dictionary<string, string> {
                { Property_Folder, folder}
            });
        }

        public void AddPodcastFeedEvent(string url)
        {
            Analytics.TrackEvent(Event_Add_Podcast_Feed, new Dictionary<string, string> {
                { Property_Url, url}
            });
        }

        public void RemovePodcastEvent(string folder)
        {
            Analytics.TrackEvent(Event_Remove_Podcast, new Dictionary<string, string> {
                { Property_Folder, folder}
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