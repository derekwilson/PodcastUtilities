using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

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
        public static string _emptyStateResource = "PodcastUtilities.Common.XML.state.xml";

        /// <summary>
        /// name of the file we keep state in
        /// </summary>
        public static string STATE_FILE_NAME = "state.xml";

        /// <summary>
        /// create a new empty state
        /// </summary>
        public XmlState()
        {
            _xmlDocument = new XmlDocument();

            Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream(_emptyStateResource);
            _xmlDocument.Load(s);

            InitialiseState();
        }

        /// <summary>
		/// create the object and read the control file from the specified filename
		/// </summary>
		/// <param name="filename">pathname to the control file xml</param>
        public XmlState(string filename)
		{
			_xmlDocument = new XmlDocument();

            try
            {
                _xmlDocument.Load(filename);
            }
            catch (XmlException)
            {
                Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream(_emptyStateResource);
                _xmlDocument.Load(s);
            }

            InitialiseState();
        }

        /// <summary>
        /// only used for unit testing
        /// </summary>
        public XmlState(XmlDocument document)
		{
		    _xmlDocument = document;

            InitialiseState();
        }

        private void InitialiseState()
        {
            _highTide = GetHighTideDate();
        }

        private string GetText(XmlNode context, string xPathSelect, string defaultValue)
        {
            if (context == null)
                return defaultValue;

            XmlNode n = context.SelectSingleNode(xPathSelect);
            if (n == null)
                return defaultValue;

            return n.InnerText;
        }

        private int GetInt(XmlNode context, string xPathSelect, int defaultValue)
        {
            int retval = defaultValue;
            try
            {
                retval = Convert.ToInt32(GetText(context, xPathSelect, retval.ToString()));
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
        private System.DateTime GetDate(XmlNode node)
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

        private void SetDate(XmlNode xmlNode, DateTime highTide)
        {
            SetText(xmlNode, "year", highTide.Year.ToString());
            SetText(xmlNode, "month", highTide.Month.ToString());
            SetText(xmlNode, "day", highTide.Day.ToString());
            SetText(xmlNode, "hour", highTide.Hour.ToString());
            SetText(xmlNode, "minute", highTide.Minute.ToString());
            SetText(xmlNode, "second", highTide.Second.ToString());
        }

        private void SetText(XmlNode xmlNode, string xPath, string text)
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
            _xmlDocument.Save(Path.Combine(folder,STATE_FILE_NAME));
        }
    }
}
