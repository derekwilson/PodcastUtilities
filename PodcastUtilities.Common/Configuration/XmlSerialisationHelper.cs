﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

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
    public static class XmlSerializationHelper
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

        /// <summary>
        /// deep copy an object using XmlSerialization
        /// </summary>
        /// <param name="elementName">name of surrounding element when writing the object</param>
        /// <param name="source">object to copy</param>
        /// <param name="destination">new blank element to copy into</param>
        public static void CloneUsingXmlSerialization(string elementName, IXmlSerializable source, IXmlSerializable destination)
        {
            XmlWriterSettings writeSettings = new XmlWriterSettings
            {
                OmitXmlDeclaration = true,
                ConformanceLevel = ConformanceLevel.Fragment,
                CloseOutput = false,
                Encoding = Encoding.UTF8
            };

            MemoryStream memoryStream = new MemoryStream();
            var xmlWriter = XmlWriter.Create(memoryStream, writeSettings);

            // simulate the behaviour of XmlSerialisation
            xmlWriter.WriteStartElement(elementName);
            source.WriteXml(xmlWriter);
            xmlWriter.WriteEndElement();

            xmlWriter.Flush();
            memoryStream.Position = 0;

            XmlReaderSettings readSettings = new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Fragment };
            var reader = XmlReader.Create(memoryStream, readSettings);

            destination.ReadXml(reader);
        }
    }
}
