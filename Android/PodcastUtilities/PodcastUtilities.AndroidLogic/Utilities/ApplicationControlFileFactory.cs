using PodcastUtilities.Common.Configuration;
using System.Xml;

namespace PodcastUtilities.AndroidLogic.Utilities
{
    public interface IApplicationControlFileFactory
    {
        IReadWriteControlFile CreateControlFile(XmlDocument xml);
    }
    public class ApplicationControlFileFactory : IApplicationControlFileFactory
    {
        public IReadWriteControlFile CreateControlFile(XmlDocument xml)
        {
            return new ReadWriteControlFile(xml);
        }
    }
}