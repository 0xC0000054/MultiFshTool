using System;
using System.Globalization;
using System.Xml;

namespace loaddatfsh
{
    internal class Settings
    {
        XmlDocument xmlDocument;
                
        string documentPath;

        public Settings(string path)
        {
            try 
            {
                documentPath = path;
                xmlDocument = new XmlDocument();
                xmlDocument.Load(documentPath);
            }
            catch { xmlDocument.LoadXml("<settings></settings>"); }
        }

        public int GetSetting(string xPath, int defaultValue)
        { 
            return Convert.ToInt16(GetSetting(xPath, defaultValue.ToString(CultureInfo.InvariantCulture)), CultureInfo.InvariantCulture);
        }

        public void PutSetting(string xPath, int value)
        { 
            PutSetting(xPath, value.ToString(CultureInfo.InvariantCulture)); 
        }

        public string GetSetting(string xPath, string defaultValue)
        {
            XmlNode xmlNode = xmlDocument.SelectSingleNode("settings/" + xPath);
            if (xmlNode != null) { return xmlNode.InnerText; }
            else { return defaultValue; }
        }

        public void PutSetting(string xPath, string value)
        {
            XmlNode xmlNode = xmlDocument.SelectSingleNode("settings/" + xPath);
            if (xmlNode == null) { xmlNode = createMissingNode("settings/" + xPath); }
            xmlNode.InnerText = value;
            xmlDocument.Save(documentPath);
        }

        private XmlNode createMissingNode(string xPath)
        {
            string[] xPathSections = xPath.Split('/');
            string currentXPath = "";
            XmlNode testNode = null;
            XmlNode currentNode = xmlDocument.SelectSingleNode("settings");
            foreach (string xPathSection in xPathSections)
            {
                currentXPath += xPathSection;
                testNode = xmlDocument.SelectSingleNode(currentXPath);
                if (testNode == null) { currentNode.InnerXml += "<" + xPathSection + "></" + xPathSection + ">"; }
                currentNode = xmlDocument.SelectSingleNode(currentXPath);
                currentXPath += "/";
            }
            return currentNode;
        }
    }
}
