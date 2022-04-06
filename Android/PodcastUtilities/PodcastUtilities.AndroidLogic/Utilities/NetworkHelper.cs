using Android.Content;
using Android.Net;
using Android.OS;
using PodcastUtilities.AndroidLogic.Logging;
using static PodcastUtilities.AndroidLogic.Utilities.INetworkHelper;

namespace PodcastUtilities.AndroidLogic.Utilities
{
    public interface INetworkHelper
    {
        enum NetworkType
        {
            None,
            Wifi,
            Cellular,
            Vpn
        }

        NetworkType ActiveNetworkType { get; }
    }
    public class NetworkHelper : INetworkHelper
    {
        private Context ApplicationContext;
        private ILogger Logger;

        public NetworkHelper(Context applicationContext, ILogger logger)
        {
            ApplicationContext = applicationContext;
            Logger = logger;
        }


        public INetworkHelper.NetworkType ActiveNetworkType
        {
            // we need to get this every time we are asked as it might change
            // so dont cache it and remember this object could be a singleton
            get
            {
                var connectivityManager = ApplicationContext.GetSystemService(Context.ConnectivityService) as ConnectivityManager;
                if (connectivityManager == null)
                {
                    Logger.Debug(() => $"NetworkHelper:ActiveNetworkType - cannot get connectivity manager");
                    return NetworkType.None;
                }
                if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
                {
                    // new mechanism introduced in SDK 23 (M)
                    NetworkCapabilities capabilities = connectivityManager.GetNetworkCapabilities(connectivityManager.ActiveNetwork);
                    if (capabilities == null)
                    {
                        return NetworkType.None;
                    }
                    else if (capabilities.HasTransport(TransportType.Wifi))
                    {
                        return NetworkType.Wifi;
                    }
                    else if (capabilities.HasTransport(TransportType.Cellular))
                    {
                        return NetworkType.Cellular;
                    }
                    else if (capabilities.HasTransport(TransportType.Vpn))
                    {
                        return NetworkType.Vpn;
                    }
                }
                else
                {
                    // old mechanism for old OS
                    // the code is obsolete
                    #pragma warning disable CS0618 // Type or member is obsolete
                    NetworkInfo activeNetwork = connectivityManager.ActiveNetworkInfo;
                    if (activeNetwork == null)
                    {
                        return NetworkType.None;
                    }
                    else if (activeNetwork.Type == ConnectivityType.Wifi)
                    {
                        return NetworkType.Wifi;
                    }
                    else if (activeNetwork.Type == ConnectivityType.Mobile)
                    {
                        return NetworkType.Cellular;
                    }
                    else if (activeNetwork.Type == ConnectivityType.Vpn)
                    {
                        return NetworkType.Vpn;
                    }
                    #pragma warning restore CS0618 // Type or member is obsolete
                }
                return NetworkType.None;
            }
        }
    }
}