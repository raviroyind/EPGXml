using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;
using XmlParser.Code;

namespace XmlParser
{

    public class XmlStripper
    {
        public XmlNode RemoveAllNamespaces(XmlNode documentElement)
        {
            var xmlnsPattern = "\\s+xmlns\\s*(:\\w)?\\s*=\\s*\\\"(?<url>[^\\\"]*)\\\"";
            var outerXml = documentElement.OuterXml;
            var matchCol = Regex.Matches(outerXml, xmlnsPattern);
            foreach (var match in matchCol)
                outerXml = outerXml.Replace(match.ToString(), "");

            var result = new XmlDocument();
            result.LoadXml(outerXml);

            return result;
        }

        public XmlNode Strip(XmlNode documentElement)
        {
            var namespaceManager = new XmlNamespaceManager(documentElement.OwnerDocument.NameTable);
            foreach (var nspace in namespaceManager.GetNamespacesInScope(XmlNamespaceScope.All))
            {
                namespaceManager.RemoveNamespace(nspace.Key, nspace.Value);
            }

            return documentElement;
        }
    }
    class MyComparer : IEqualityComparer<XElement>
    {
        public bool Equals(XElement x, XElement y)
        {
            return x.Attribute("start").Value == y.Attribute("start").Value;
        }

        public int GetHashCode(XElement obj)
        {
            return obj.Attribute("start").Value.GetHashCode();
        }
    }

    public partial class accessDenied : System.Web.UI.Page
    {
        #region Namespace...

        XNamespace xsd = "http://www.w3.org/2001/XMLSchema";
        XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
        XNamespace schemaLocation = XNamespace.Get("tv:noNamespaceSchemaLocation");

        #endregion Namespace...
        public static XmlDocument ToXmlDocument(XDocument xDocument)
        {
            var xmlDocument = new XmlDocument();
            using (var xmlReader = xDocument.CreateReader())
            {
                xmlDocument.Load(xmlReader);
            }
            return xmlDocument;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            var doc =   CommonFunctions.GetPastDataXml("W9", "W9");

            var filterList = doc.Descendants()
                .DescendantNodes()
                .OfType<XElement>()
                .Where(x => x.Name.LocalName.Equals("programme"));


            var edoc = new XDocument();
            edoc.Add(new XElement("feed", ""));
            edoc.Root.Add(filterList);

            var xElement = XElement.Parse(edoc.ToString());

            var finalList = xElement.Elements().Distinct(new MyComparer()).ToList();

            var docresult = new XDocument();
            docresult.Add(new XElement("tv",
                new XAttribute(XNamespace.Xmlns + "xsd", xsd),
                new XAttribute(XNamespace.Xmlns + "xsi", xsi)
                 , new XAttribute(xsi + "schemaLocation", schemaLocation)));

            docresult.Root.Add(new XElement("channel",
               new XAttribute("id", "W9"),
               new XAttribute("name", "W9"),
                    new XElement("display-name", "W9"))
               );

            foreach (var item in finalList)
            {
                docresult.Root.Add(new XElement("programme",
                    new XAttribute("channel", item.Attribute("channel").Value),
                    new XAttribute("start", item.Attribute("start").Value),
                    new XAttribute("stop", item.Attribute("stop").Value),
                  GetNodes(item)
                    ));
            }


            var xml = ToXmlDocument(docresult);
            var result = new XmlStripper().RemoveAllNamespaces(xml.DocumentElement);

            Response.Write(result);
        }

        private object GetNodes(XElement item)
        {
            XNode[] objListObjects = new XNode[item.Nodes().Count()];

            int i = 0;
            foreach (var node in item.Nodes())
            {
                objListObjects[i] = node;
                i++;
            }

            return objListObjects;
        }
    }
}