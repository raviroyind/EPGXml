using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace XmlParser.Secured
{
    public partial class fullxmlservice : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["USR_TYPE"] == null && Request.QueryString["token"] == null)
                Response.Redirect("../default.aspx?id=ua", true);
            else if (Convert.ToString(Session["USR_TYPE"]) == "Guest" && Request.QueryString["token"] == null)
                Response.Redirect("../accessDenied.aspx", true);


            if (Request.QueryString["channel-id"] != null)
            {
                 
                var channelService = new FullChannelWebService.FullChannelService();

                if (Request.QueryString["token"] == null)
                {
                    FullChannelWebService.AuthHeader header = new FullChannelWebService.AuthHeader
                    {
                        Username = "adminsvc@channelsvc.com",
                        Password = "I(0)1stog"
                    };

                    channelService.AuthHeaderValue = header;
                }

                try
                {
                    var hashSet = new HashSet<string>();

                    var channelIds = Request.QueryString["channel-id"];

                    if (channelIds.Contains(','))
                    {
                        var strings = channelIds.Split(',');

                        foreach (var item in strings)
                        {
                            hashSet.Add(item.Trim());
                        }
                    }
                    else
                    {
                        hashSet.Add(channelIds);
                    }

                     

                    string message;
                    var doc = channelService.GetChannels(Request.QueryString["channel-id"],Request.QueryString["format"], Request.QueryString["token"], Request.QueryString["length"], hashSet.Count > 1,out message);

                   
                    if (doc != null)
                    {
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
                    else
                    {
                        Response.Write(message);
                    }
                }
                catch (Exception exception)
                {

                }
            }
        }
    }
     
}
