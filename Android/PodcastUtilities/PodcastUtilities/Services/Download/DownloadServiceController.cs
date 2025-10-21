using Android.Content;
using Android.OS;
using PodcastUtilities.AndroidLogic.Logging;
using PodcastUtilities.AndroidLogic.Services.Download;

namespace PodcastUtilities.Services.Download
{
    public class DownloadServiceController : Java.Lang.Object, IServiceConnection, IDownloadServiceController
    {
        private ILogger Logger;
        private Context ApplicationContext;

        public DownloadServiceController(ILogger logger, Context applicationContext)
        {
            Logger = logger;
            ApplicationContext = applicationContext;
        }

        // there is only one listener so do not make the controller a singleton - we need a new object every time
        private IDownloadServiceConnectionListener? connectionListener = null;

        private Intent GetServiceIntent()
        {
            return new Intent(ApplicationContext, typeof(DownloadService));
        }

        public void StartService()
        {
            Logger.Debug(() => $"DownloadServiceController:StartService");
            ApplicationContext.StartService(GetServiceIntent());
        }

        public void StopService()
        {
            Logger.Debug(() => $"DownloadServiceController:StopService");
            ApplicationContext.StopService(GetServiceIntent());
        }

        public void BindToService(IDownloadServiceConnectionListener listener)
        {
            Logger.Debug(() => $"DownloadServiceController:BindToService");
            connectionListener = listener;
            ApplicationContext.BindService(GetServiceIntent(), this, Bind.AutoCreate);
        }

        public void UnbindFromService()
        {
            Logger.Debug(() => $"DownloadServiceController:UnbindFromService");
            ApplicationContext.UnbindService(this);
        }

        public void OnServiceConnected(ComponentName? name, IBinder? service)
        {
            Logger.Debug(() => $"DownloadServiceController:OnServiceConnected - {name}");
            var binder = service as DownloadServiceBinder;
            if (connectionListener != null && binder != null)
            {
                connectionListener.ConnectService(binder.service);
            } 
            else
            {
                Logger.Warning(() => $"DownloadServiceController:OnServiceConnected - cannot bind");
            }
        }

        public void OnServiceDisconnected(ComponentName? name)
        {
            Logger.Debug(() => $"DownloadServiceController:OnServiceDisconnected - {name}");
            if (connectionListener != null)
            {
                connectionListener.DisconnectService();
            }
        }
    }
}