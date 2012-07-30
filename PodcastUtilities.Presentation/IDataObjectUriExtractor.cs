using System.Windows;

namespace PodcastUtilities.Presentation
{
    public interface IDataObjectUriExtractor
    {
        bool ContainsUri(IDataObject dataObject);

        string GetUri(IDataObject dataObject);
    }
}