using PodcastUtilities.Common.Configuration;
using System.Xml;

namespace PodcastUtilities.AndroidLogic.Utilities
{
    public interface IApplicationControlFileFactory
    {
        IReadWriteControlFile CreateControlFile(XmlDocument xml);
        IReadWriteControlFile CreateEmptyControlFile();
    }
    public class ApplicationControlFileFactory : IApplicationControlFileFactory
    {
        public IReadWriteControlFile CreateControlFile(XmlDocument xml)
        {
            return new ReadWriteControlFile(xml);
        }

        public IReadWriteControlFile CreateEmptyControlFile()
        {
            return new ReadWriteControlFile();
        }
    }
}