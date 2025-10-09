namespace PodcastUtilities.AndroidLogic.Services.Download
{
    public interface IDownloadServiceConnectionListener
    {
        void ConnectService(IDownloadService service);
        void DisconnectService();
    }


    public interface IDownloadServiceController
    {
        void StartService();
        void StopService();
        void BindToService(IDownloadServiceConnectionListener listener);
        void UnbindFromService();
    }
}