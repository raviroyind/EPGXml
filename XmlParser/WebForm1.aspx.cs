using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using XmlParser.Core.DataContext;

namespace XmlParser
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        #region Namespace...

        XNamespace xsd = "http://www.w3.org/2001/XMLSchema";
        XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
        XNamespace schemaLocation = XNamespace.Get("tv:noNamespaceSchemaLocation");

        #endregion Namespace...
        protected void Page_Load(object sender, EventArgs e)
        {
            var dInfo = new DirectoryInfo(ConfigurationManager.AppSettings["PastXmlFolderPath"] + @"Day1\");

            var files = dInfo.GetFiles("*.xml", SearchOption.TopDirectoryOnly);

            foreach (var file in files)
            {
                var elems = GetChannelTable(file.FullName);


                var doc = new XDocument();

                doc.Add(new XElement("tv", ""));

                doc.Root.Add(new XElement("channel",
                    new XAttribute("id", Path.GetFileNameWithoutExtension(file.Name)),
                    new XAttribute("name", Path.GetFileNameWithoutExtension(file.Name))
                    ));

                doc.Root.Add(elems);

                doc.Save(ConfigurationManager.AppSettings["PastXmlFolderPath"] + @"12Output\"+file.Name);
            }
 
        }

        private static List<XElement> GetChannelTable(string file)
        {
            //start = "2016212
                    List<XElement> filterList=new List<XElement>();

                    try
                    {
                        var document = XDocument.Load(file);
                            filterList = document.Descendants()
                                .DescendantNodes()
                                .OfType<XElement>()
                                .Where(x => x.Name.LocalName.Equals("programme"))
                                .Where(x => x.Attribute("start").Value.Substring(0,7).Equals("2016212")).ToList();


                    }
                    catch (FileNotFoundException feException)
                    {  }
             
            return filterList;
        }
    }
}