using System;
using System.Xml.Serialization;

namespace PodcastUtilities.Common.Configuration
{
    /// <summary>
    /// an external command
    /// </summary>
    public interface ITokenisedCommand : ICloneable, IXmlSerializable
    {
        /// <summary>
        /// the command
        /// </summary>
        IDefaultableItem<string> Command { get; set; }

        /// <summary>
        /// the arguments
        /// </summary>
        IDefaultableItem<string> Arguments { get; set; }

        /// <summary>
        /// the current working dir
        /// </summary>
        IDefaultableItem<string> WorkingDirectory { get; set; }
    }
}