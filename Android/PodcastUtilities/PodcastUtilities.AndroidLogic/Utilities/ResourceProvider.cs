using Android.Content;

namespace PodcastUtilities.AndroidLogic.Utilities
{
    public interface IResourceProvider
    {
        string GetString(int id);
        string GetQuantityString(int id, int quantity);
    };

    public class AndroidResourceProvider : IResourceProvider
    {
        private Context ResourceContext;

        public AndroidResourceProvider(Context context)
        {
            ResourceContext = context;
        }

        public string GetQuantityString(int id, int quantity)
        {
            return ResourceContext.Resources.GetQuantityString(id, quantity, quantity);
        }

        public string GetString(int id)
        {
            return ResourceContext.GetString(id);
        }

    }
}