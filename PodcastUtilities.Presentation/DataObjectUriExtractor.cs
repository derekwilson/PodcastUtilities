using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace PodcastUtilities.Presentation
{
    public class DataObjectUriExtractor : IDataObjectUriExtractor
    {
        #region Implementation of IDataObjectUriExtractor

        public bool ContainsUri(IDataObject dataObject)
        {
            return (GetUri(dataObject) != null);
        }

        public string GetUri(IDataObject dataObject)
        {
            var uriData = GetDataInUrlFormat(dataObject);

            return uriData ?? GetDataInTextFormat(dataObject);
        }

        #endregion

        private static string GetDataInUrlFormat(IDataObject dataObject)
        {
            var stream = (MemoryStream)dataObject.GetData("UniformResourceLocator");
            if (stream == null)
            {
                return null;
            }

            var terminatedStringBytes = stream.ToArray().TakeWhile(b => b != 0);

            return Encoding.ASCII.GetString(terminatedStringBytes.ToArray());
        }

        private static string GetDataInTextFormat(IDataObject dataObject)
        {
            var text = dataObject.GetData("Text") as string;

            return (IsValidUri(text) ? text : null);
        }

        private static bool IsValidUri(string address)
        {
            return ((address != null) && Uri.IsWellFormedUriString(address, UriKind.Absolute));
        }
    }
}