#region License
// FreeBSD License
// Copyright (c) 2010 - 2013, Andrew Trevarrow and Derek Wilson
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
// Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
// 
// Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED 
// WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A 
// PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR
// ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
// TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
// POSSIBILITY OF SUCH DAMAGE.
#endregion
using System.Xml;
using System.Xml.Schema;

namespace PodcastUtilities.Common.Configuration
{
    /// <summary>
    /// a tokenised command
    /// </summary>
    public class TokenisedCommand : ITokenisedCommand
    {
        private readonly IControlFileGlobalDefaults _controlFileGlobalDefaults;

        /// <summary>
        /// a tokenised command
        /// </summary>
        /// <param name="controlFileGlobalDefaults"></param>
        public TokenisedCommand(IControlFileGlobalDefaults controlFileGlobalDefaults)
        {
            _controlFileGlobalDefaults = controlFileGlobalDefaults;
            Command = new DefaultableReferenceTypeItem<string>(_controlFileGlobalDefaults.GetDefaultPostDownloadCommand);
            Arguments = new DefaultableReferenceTypeItem<string>(_controlFileGlobalDefaults.GetDefaultPostDownloadArguments);
            WorkingDirectory = new DefaultableReferenceTypeItem<string>(_controlFileGlobalDefaults.GetDefaultPostDownloadWorkingDirectory);
        }

        /// <summary>
        /// the command
        /// </summary>
        public IDefaultableItem<string> Command { get; set; }

        /// <summary>
        /// the arguments
        /// </summary>
        public IDefaultableItem<string> Arguments { get; set; }

        /// <summary>
        /// the current working dir
        /// </summary>
        public IDefaultableItem<string> WorkingDirectory { get; set; }


        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public object Clone()
        {
            var copy = new TokenisedCommand(_controlFileGlobalDefaults);
            XmlSerializationHelper.CloneUsingXmlSerialization("postdownloadcommand", this, copy);
            return copy;
        }

        /// <summary>
        /// This method is reserved and should not be used. When implementing the IXmlSerializable interface, you should return null (Nothing in Visual Basic) from this method, and instead, if specifying a custom schema is required, apply the <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute"/> to the class.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Xml.Schema.XmlSchema"/> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)"/> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)"/> method.
        /// </returns>
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader"/> stream from which the object is deserialized. 
        ///                 </param>
        public void ReadXml(XmlReader reader)
        {
            XmlSerializationHelper.ProcessElement(reader, "postdownloadcommand", ProcessTokenisedCommandElements);
        }

        private ProcessorResult ProcessTokenisedCommandElements(XmlReader reader)
        {
            var result = ProcessorResult.Processed;

            var elementName = reader.LocalName;
            if (!reader.IsEmptyElement)
            {
                reader.Read();
            }
            var content = reader.Value.Trim();
            switch (elementName)
            {
                case "command":
                    if (!string.IsNullOrEmpty(content))
                    {
                        Command.Value = content;
                    }
                    break;
                case "arguments":
                    if (!string.IsNullOrEmpty(content))
                    {
                        Arguments.Value = content;
                    }
                    break;
                case "workingdirectory":
                    if (!string.IsNullOrEmpty(content))
                    {
                        WorkingDirectory.Value = content;
                    }
                    break;
                default:
                    result = ProcessorResult.Ignored;
                    break;

            }
            return result;
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter"/> stream to which the object is serialized. 
        ///                 </param>
        public void WriteXml(XmlWriter writer)
        {
            if (Command.IsSet)
            {
                writer.WriteElementString("command", Command.Value);
            }
            if (Arguments.IsSet)
            {
                writer.WriteElementString("arguments", Arguments.Value);
            }
            if (WorkingDirectory.IsSet)
            {
                writer.WriteElementString("workingdirectory", WorkingDirectory.Value);
            }
        }
    }
}