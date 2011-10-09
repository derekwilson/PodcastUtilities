using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace PodcastUtilities.Common.Configuration
{
    /// <summary>
    /// the resullt of processing an element or a sub element
    /// </summary>
    public enum ProcessorResult
    {
        /// <summary>
        /// the element was ignored
        /// </summary>
        Ignored,
        /// <summary>
        /// the element was processed
        /// </summary>
        Processed,
    }

    /// <summary>
    /// a function that can process a sub element
    /// </summary>
    /// <param name="reader">the reader</param>
    public delegate ProcessorResult SubElementProcessor(XmlReader reader);

    /// <summary>
    /// helper to process sections of an xml file
    /// </summary>
    public class XmlSerializationHelper
    {
        /// <summary>
        /// process an xml element when reading an xml stream
        /// </summary>
        /// <param name="reader">the reader</param>
        /// <param name="elementName">the name of the element we are processing</param>
        /// <param name="subElementProcessor">a delegate to process any sub elements detected</param>
        public static void ProcessElement(XmlReader reader, string elementName, SubElementProcessor subElementProcessor)
        {
            if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == elementName)
            {
                reader.Read(); // Skip ahead to next node
                var element = reader.MoveToContent();
                while (element != XmlNodeType.None)
                {
                    if (element == XmlNodeType.EndElement && reader.LocalName == elementName)
                    {
                        break;
                    }
                    if (reader.IsStartElement())
                    {
                        subElementProcessor(reader);
                    }
                    reader.Read();
                    element = reader.MoveToContent();
                }
            }
        }
    }
}
