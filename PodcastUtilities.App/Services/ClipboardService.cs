using System.Windows;
using PodcastUtilities.Presentation.Services;

namespace PodcastUtilities.App.Services
{
    public class ClipboardService
        : IClipboardService
    {
        public string GetText()
        {
            return Clipboard.GetText();
        }
    }
}