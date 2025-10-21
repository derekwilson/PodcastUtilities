using Android.Graphics.Drawables;
using Android.Text;
using PodcastUtilities.AndroidLogic.Logging;

namespace PodcastUtilities.AndroidLogic.Utilities
{
    public class ImageGetter : Java.Lang.Object, Html.IImageGetter
    {
        private IResourceProvider ResourceProvider;
        private ILogger Logger;

        public ImageGetter(
            IResourceProvider resourceProvider,
            ILogger logger
        )
        {
            ResourceProvider = resourceProvider;
            Logger = logger;
        }

        public Drawable? GetDrawable(string? source)
        {
            int id;

            Logger.Debug(() => $"ImageGetter:GetDrawable - {source}");

            switch (source)
            {
                case "rss":
                    id = Resource.Drawable.orange_rss_small;
                    break;
                default:
                    return null;
            }

            Drawable? d = ResourceProvider.GetDrawable(id);
            d?.SetBounds(0, 0, d.IntrinsicWidth, d.IntrinsicHeight);
            return d;
        }
    }
}