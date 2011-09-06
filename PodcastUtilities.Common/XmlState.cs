using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace PodcastUtilities.Common
{
    /// <summary>
    /// provides state information
    /// </summary>
    public class XmlState : IState
    {
        private readonly XmlDocument _xmlDocument;

        private DateTime _highTide = DateTime.MinValue;

        /// <summary>
        /// resource path to the WPL template
        /// </summary>
        public const string EmptyStateResource = "PodcastUtilities.Common.XML.state.xml";

        /// <summary>
        /// name of the file we keep state in
        /// </summary>
        public const string StateFileName = "state.xml";

        /// <summary>
        /// create a new empty state
        /// </summary>
        public XmlState()
        {
            _xmlDocument = new XmlDocument();

            Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream(EmptyStateResource);
            _xmlDocument.Load(s);

            InitialiseState();
        }

        /// <summary>
		/// create the object and read the control file from the specified filename
		/// </summary>
		/// <param name="fileName">pathname to the control file xml</param>
        public XmlState(string fileName)
		{
			_xmlDocument = new XmlDocument();

            try
            {
                _xmlDocument.Load(fileName);
            }
            catch (XmlException)
            {
                _xmlDocument.Load(Assembly.GetExecutingAssembly().GetManifestResourceStream(EmptyStateResource));
            }

            InitialiseState();
        }

        /// <summary>
        /// only used for unit testing
        /// </summary>
        public XmlState(IXPathNavigable document)
		{
            _xmlDocument = new XmlDocument();
            _xmlDocument.InnerXml = document.CreateNavigator().InnerXml;

            InitialiseState();
        }

        private void InitialiseState()
        {
            _highTide = GetHighTideDate();
        }

        private static string GetText(XmlNode context, string xPathSelect, string defaultValue)
        {
            if (context == null)
                return defaultValue;

            XmlNode n = context.SelectSingleNode(xPathSelect);
            if (n == null)
                return defaultValue;

            return n.InnerText;
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private static int GetInt(XmlNode context, string xPathSelect, int defaultValue)
        {
            int retval = defaultValue;
            try
            {
                retval = Convert.ToInt32(GetText(context, xPathSelect, retval.ToString(CultureInfo.InvariantCulture)), CultureInfo.InvariantCulture);
            }
            catch
            {
                retval = defaultValue;
            }
            return retval;
        }

        /// <summary>
        /// convert an XML tree into a datetime
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private static System.DateTime GetDate(XmlNode node)
        {
            System.DateTime retval;
            try
            {
                retval = new System.DateTime(
                    GetInt(node, "year",0),
                    GetInt(node, "month",0),
                    GetInt(node, "day",0),
                    GetInt(node, "hour",0),
                    GetInt(node, "minute",0),
                    GetInt(node, "second",0)
                    );
            }
            catch
            {
                retval = System.DateTime.MinValue;
            }
            return retval;
        }

        private System.DateTime GetHighTideDate()
        {
            return GetDate(_xmlDocument.SelectSingleNode("state/highTide"));
        }

        private void SetHighTideDate(DateTime highTide)
        {
            SetDate(_xmlDocument.SelectSingleNode("state/highTide"),highTide);
        }

        private static void SetDate(XmlNode xmlNode, DateTime highTide)
        {
            SetText(xmlNode, "year", highTide.Year.ToString(CultureInfo.InvariantCulture));
            SetText(xmlNode, "month", highTide.Month.ToString(CultureInfo.InvariantCulture));
            SetText(xmlNode, "day", highTide.Day.ToString(CultureInfo.InvariantCulture));
            SetText(xmlNode, "hour", highTide.Hour.ToString(CultureInfo.InvariantCulture));
            SetText(xmlNode, "minute", highTide.Minute.ToString(CultureInfo.InvariantCulture));
            SetText(xmlNode, "second", highTide.Second.ToString(CultureInfo.InvariantCulture));
        }

        private static void SetText(XmlNode xmlNode, string xPath, string text)
        {
            XmlNode n = xmlNode.SelectSingleNode(xPath);
            n.InnerText = text;
        }

        /// <summary>
        /// the latest publish date for a downloaded podcast
        /// </summary>
        public DateTime DownloadHighTide
        {
            get
            {
                return _highTide;
            }
            set
            {
                if (value > _highTide)
                {
                    _highTide = value;
                    SetHighTideDate(_highTide);
                }
            }
        }

        /// <summary>
        /// persist the state
        /// </summary>
        public void SaveState(string folder)
        {
            _xmlDocument.Save(Path.Combine(folder,StateFileName));
        }
    }
}
