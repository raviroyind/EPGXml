using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Xml;
using System.Xml.Linq;
using XmlParser.Code;
using XmlParser.Core.DataContext;

namespace XmlParser.Secured
{
    public partial class XmlTransformation : Page
    {
        #region Namespace...

        readonly XNamespace _xsd = "http://www.w3.org/2001/XMLSchema";
        readonly XNamespace _xsi = "http://www.w3.org/2001/XMLSchema-instance";
        readonly XNamespace _schemaLocation = XNamespace.Get("tv:noNamespaceSchemaLocation");

        #endregion Namespace...
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.Url.Host.Contains("admin.latvf.net") && string.IsNullOrEmpty(Request.QueryString["parent"]))
            {
                Response.Redirect("default.aspx", true);
            }
             
            try
            {
                 
                var file = Request.Url.Query;
                var xmlPath = string.Empty;

                //Multiple Conditions to handle when url is pasted into browser.
                if (!file.Contains("+"))
                    xmlPath = Request.QueryString["file"];
                else if (file.Contains("+%20"))
                {
                    var start = file.LastIndexOf("/", StringComparison.Ordinal) + 1;
                    var end = file.IndexOf(".xml", start, StringComparison.Ordinal) + 4;
                    var result = file.Substring(start, end - start);
                    result = result.Replace("+", "%2b");
                    
                    result = HttpUtility.UrlDecode(result);

                    xmlPath = "../Output/" + result;
                }
                else if (file.Contains("+"))
                {
                    if(file.Contains("%2f"))
                    {
                        var start = file.LastIndexOf("%2f", StringComparison.Ordinal) + 3;
                        var end = file.IndexOf(".xml", start, StringComparison.Ordinal) + 4;
                        var result = file.Substring(start, end - start);
                        result = HttpUtility.UrlDecode(result);
                        xmlPath = "../Output/" + result;
                    }
                    else if (file.Contains("/"))
                    {
                        var start = file.LastIndexOf("/", StringComparison.Ordinal) + 1;
                        var end = file.IndexOf(".xml", start, StringComparison.Ordinal) + 4;
                        var result = file.Substring(start, end - start);
                        result = HttpUtility.UrlDecode(result);
                        xmlPath = "../Output/" + result;
                    }
                }
                 
                Display(xmlPath,
                    Request.QueryString["channelname"],
                         Request.QueryString["offset"]!=null ? Convert.ToInt32(Request.QueryString["offset"]) : 0);
            }
            catch (Exception ex)
            {
                string msg = "";
                msg = ex.Message;

                if(ex.InnerException!=null)
                {
                    msg += Environment.NewLine + ex.InnerException.Message;
                }
                throw new Exception(msg);
                //Session.Add("EXP", ex);
                //Session.Add("URL", Request.Url.ToString());
                //Response.Redirect("../Errors/oops.aspx");
            }
        }

        /// <summary>
        /// Renders Xml document to response changing "channelName" & "offset" values if supplied.
        /// </summary>
        /// <param name="file">File to be rendered.</param>
        /// <param name="channelName">Channel name which will replace the existing name.</param>
        /// <param name="offset">Hoours Offset value.</param>
        void Display(string file, string channelName = null, int offset = 0)
        {
            var finalDoc = new XDocument();
            if (!string.IsNullOrEmpty(file) && File.Exists(Server.MapPath(file)))
            {

                var channelId = Path.GetFileNameWithoutExtension(file);

                //var dataContext = new EPGDataModel();
                
                //dataContext.Database.Connection.ConnectionString = ConfigurationManager.ConnectionStrings["EPGDataModel"].ConnectionString;

                //    var fileNm = Path.GetFileName(file);
                //    var firstOrDefault = dataContext.XmlImports.Where(c => c.XmlFileName.Equals(fileNm)).FirstOrDefault();
                //    if (firstOrDefault != null)
                //        channelId =
                //            firstOrDefault.ChannelId;
                


                var doc = XDocument.Load(Server.MapPath(file));
                 

                finalDoc = XDocument.Load(Server.MapPath(file));
                
#region Added for Past Xml...

                if (File.Exists(ConfigurationManager.AppSettings["PastXmlFolderPath"] + @"\Past7\" + Path.GetFileName(file)))
                {
                    var docPastXml =
                    XDocument.Load(ConfigurationManager.AppSettings["PastXmlFolderPath"] + @"\Past7\" +
                                   Path.GetFileName(file));

                    docPastXml.Root.Add(doc.Descendants()
                       .DescendantNodes()
                       .OfType<XElement>()
                       .Where(n => n.Name.LocalName.Equals("programme")));


                    var filterList = docPastXml.Descendants()
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
                        new XAttribute(XNamespace.Xmlns + "xsd", _xsd),
                        new XAttribute(XNamespace.Xmlns + "xsi", _xsi)
                         , new XAttribute(_xsi + "schemaLocation", _schemaLocation)));

                    docresult.Root.Add(new XElement("channel",
                       new XAttribute("id", channelId
                           
                           
                           ),
                       new XAttribute("name", Path.GetFileNameWithoutExtension(file)),
                            new XElement("display-name", Path.GetFileNameWithoutExtension(file)))
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

                    finalDoc = ToXDocument(result);
                     
                }
                

                 

#endregion Added for Past Xml...

                if (!string.IsNullOrEmpty(channelName))
                {

                    foreach (var att in finalDoc.Descendants()
                        .DescendantNodes()
                        .OfType<XElement>()
                        .Where(x => x.Name.LocalName.Equals("channel"))
                        .Attributes("id"))
                    {
                        att.Value = Request.QueryString["channelname"];
                    }


                    foreach (var att in finalDoc.Descendants()
                        .DescendantNodes()
                        .OfType<XElement>()
                        .Where(x => x.Name.LocalName.Equals("programme"))
                        .Attributes("channel"))
                    {
                        att.Value = Request.QueryString["channelname"];
                    }
                }


                if (offset != 0)
                {
                    foreach (var att in finalDoc.Descendants()
                        .DescendantNodes()
                        .OfType<XElement>()
                        .Where(x => x.Name.LocalName.Equals("programme"))
                        .Attributes("start"))
                    {
                        att.Value = CommonFunctions.GetDateAddingOffset(att.Value, offset);
                    }

                    foreach (var att in finalDoc.Descendants()
                        .DescendantNodes()
                        .OfType<XElement>()
                        .Where(x => x.Name.LocalName.Equals("programme"))
                        .Attributes("stop"))
                    {
                        att.Value = CommonFunctions.GetDateAddingOffset(att.Value, offset);
                    }
                }

                finalDoc.Save(Server.MapPath(@"../temp/output.xml"));

                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.ContentType = "application/xml";
                Response.ContentEncoding = Encoding.UTF8;
                Response.WriteFile(Server.MapPath(@"../temp/output.xml"));
                Response.Flush();
                Response.End();
            }
            else
            {
                Session.Add("EXP", new Exception("File "+Server.MapPath(file)+ " does not exists."));
                Session.Add("URL", Request.Url.ToString());
                Response.Redirect("../Errors/oops.aspx");
            }
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

        public static XmlDocument ToXmlDocument(XDocument xDocument)
        {
            var xmlDocument = new XmlDocument();
            using (var xmlReader = xDocument.CreateReader())
            {
                xmlDocument.Load(xmlReader);
            }
            return xmlDocument;
        }

        public static XDocument ToXDocument(XmlDocument xmlDocument)
        {
            using (var nodeReader = new XmlNodeReader(xmlDocument))
            {
                nodeReader.MoveToContent();
                return XDocument.Load(nodeReader);
            }
        }
    }

    public class XmlStripper
    {
        public XmlDocument RemoveAllNamespaces(XmlNode documentElement)
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
    public class MyComparer : IEqualityComparer<XElement>
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
}