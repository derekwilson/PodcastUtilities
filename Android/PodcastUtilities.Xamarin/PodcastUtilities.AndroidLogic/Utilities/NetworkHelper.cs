using Android.Content;
using Android.Net;
using Android.OS;
using PodcastUtilities.AndroidLogic.Logging;
using System.Net.Security;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using static PodcastUtilities.AndroidLogic.Utilities.INetworkHelper;
using System;

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
        IPAddress ActiveNetworkAddress { get; }

        void SetNetworkConnectionLimit(int maxNumberOfConnections);
        void SetApplicationDefaultCertificateValidator();
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

        public IPAddress ActiveNetworkAddress
        {
            get
            {
                try
                {
                    var connectivityManager = ApplicationContext.GetSystemService(Context.ConnectivityService) as ConnectivityManager;
                    if (connectivityManager == null)
                    {
                        Logger.Debug(() => $"NetworkHelper:ActiveNetworkType - cannot get connectivity manager");
                        return IPAddress.Parse("127.0.0.1");
                    }
                    var linkProperties = connectivityManager.GetLinkProperties(connectivityManager.ActiveNetwork);
                    if (linkProperties != null && linkProperties.LinkAddresses.Count > 0)
                    {
                        foreach (var thisAddress in linkProperties.LinkAddresses)
                        {
                            if (thisAddress != null && thisAddress.Address.HostAddress.Contains("."))
                            {
                                Logger.Debug(() => $"NetworkHelper:ActiveNetworkAddress - {thisAddress.Address.HostAddress}");
                                return IPAddress.Parse(thisAddress.Address.HostAddress);
                            }
                        }
                    }
                    return IPAddress.Parse("127.0.0.1");
                }
                catch (Exception ex)
                {
                    Logger.LogException(() => $"NetworkHelper:ActiveNetworkAddress", ex);
                    return IPAddress.Parse("127.0.0.1");
                }
            }
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
                    else if (capabilities.HasTransport(Android.Net.TransportType.Wifi))
                    {
                        return NetworkType.Wifi;
                    }
                    else if (capabilities.HasTransport(Android.Net.TransportType.Cellular))
                    {
                        return NetworkType.Cellular;
                    }
                    else if (capabilities.HasTransport(Android.Net.TransportType.Vpn))
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

        public static bool ValidatorToIgnoreAllCertificateErrors(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // we are completely permissive of certificate errors
            return true;
        }

        public void SetApplicationDefaultCertificateValidator()
        {
            // see https://www.mono-project.com/archived/usingtrustedrootsrespectfully/
            ServicePointManager.ServerCertificateValidationCallback = ValidatorToIgnoreAllCertificateErrors;
        }

        public void SetNetworkConnectionLimit(int maxNumberOfConnections)
        {
            ServicePointManager.DefaultConnectionLimit = maxNumberOfConnections;
        }
    }
}