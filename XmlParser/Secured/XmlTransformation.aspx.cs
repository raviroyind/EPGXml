using System;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Xml.Linq;
using XmlParser.Code;

namespace XmlParser.Secured
{
    public partial class XmlTransformation : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Display(Request.QueryString["file"],
                Request.QueryString["channelname"],
                !string.IsNullOrEmpty(Request.QueryString["offset"]) ? Convert.ToInt32(Request.QueryString["offset"]) : 0);
        }

        /// <summary>
        /// Resders Xml document to response changing "channelName" & "offset" values if supplied.
        /// </summary>
        /// <param name="file">File to be rendered.</param>
        /// <param name="channelName">Channel name which will replace the existing name.</param>
        /// <param name="offset">Hoours Offset value.</param>
        void Display(string file, string channelName = null, int offset = 0)
        {
            if (!string.IsNullOrEmpty(file))
            {
                var doc = XDocument.Load(Server.MapPath(file));

                if (!string.IsNullOrEmpty(channelName))
                {
                    foreach (var att in doc.Descendants()
                        .DescendantNodes()
                        .OfType<XElement>()
                        .Where(x => x.Name.LocalName.Equals("programme"))
                        .Attributes("channel"))
                    {
                        att.Value = Request.QueryString["channelname"];
                    }
                }


                if (offset > 0)
                {
                    foreach (var att in doc.Descendants()
                        .DescendantNodes()
                        .OfType<XElement>()
                        .Where(x => x.Name.LocalName.Equals("programme"))
                        .Attributes("start"))
                    {
                        att.Value = CommonFunctions.GetDateAddingOffset(att.Value, offset);
                    }

                    foreach (var att in doc.Descendants()
                        .DescendantNodes()
                        .OfType<XElement>()
                        .Where(x => x.Name.LocalName.Equals("programme"))
                        .Attributes("stop"))
                    {
                        att.Value = CommonFunctions.GetDateAddingOffset(att.Value, offset);
                    }
                }

                doc.Save(Server.MapPath(@"../temp/output.xml"));

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
        }
    }
}