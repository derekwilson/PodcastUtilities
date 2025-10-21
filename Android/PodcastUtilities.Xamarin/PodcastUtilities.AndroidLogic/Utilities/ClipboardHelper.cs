using Android.Content;
using Android.Content.Res.Loader;
using PodcastUtilities.AndroidLogic.Logging;
using System;
using System.Drawing.Drawing2D;
using Xamarin.Essentials;

namespace PodcastUtilities.AndroidLogic.Utilities
{
    public interface IClipboardHelper
    {
        string GetUrlIfAvailable();
    }

    public class ClipboardHelper : IClipboardHelper
    {
        private ILogger Logger;
        private ClipboardManager ClipboardManager;

        public ClipboardHelper(ClipboardManager clipboardManager, ILogger logger)
        {
            ClipboardManager = clipboardManager;
            Logger = logger;
        }

        public string GetUrlIfAvailable()
        {
            if (!ClipboardManager.HasPrimaryClip)
            {
                Logger.Debug(() => $"ClipboardHelper:GetUrlIfAvailable - no primary clip");
                return null;
            }
            var description = ClipboardManager.PrimaryClipDescription;
            if (description == null)
            {
                Logger.Debug(() => $"ClipboardHelper:GetUrlIfAvailable - no primary clip description");
                return null;
            }
            if (!description.HasMimeType(ClipDescription.MimetypeTextPlain))
            {
                Logger.Debug(() => $"ClipboardHelper:GetUrlIfAvailable - no plain text");
                return null;
            }
            var item = ClipboardManager.PrimaryClip.GetItemAt(0);
            var clipText = item.Text;
            Logger.Debug(() => $"ClipboardHelper:GetUrlIfAvailable - {clipText}");
            if (string.IsNullOrWhiteSpace(clipText))
            {
                return null;
            }
            if (!Uri.IsWellFormedUriString(clipText, UriKind.Absolute))
            {
                Logger.Debug(() => $"ClipboardHelper:GetUrlIfAvailable - bad url");
            }
            return clipText;
        }
    }
}