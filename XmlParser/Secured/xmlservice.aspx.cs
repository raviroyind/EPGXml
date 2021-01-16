using System;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using XmlParser.ChannelWebService;
using XmlParser.Service;

namespace XmlParser.Secured
{
    public partial class Xmlservice : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["USR_TYPE"] == null && Request.QueryString["token"]==null)
                Response.Redirect("../default.aspx?id=ua",true);
            else if (Convert.ToString(Session["USR_TYPE"]) == "Guest" && Request.QueryString["token"] == null)
                Response.Redirect("../accessDenied.aspx", true);


            if (Request.QueryString["date"] != null && Request.QueryString["channel-id"] !=null)
            {
                var date = Convert.ToDateTime(Request.QueryString["date"]);
                
                var channelService = new ChannelWebService.ChannelService();

                if (Request.QueryString["token"] == null)
                {
                    ChannelWebService.AuthHeader header = new ChannelWebService.AuthHeader
                    {
                        Username = "adminsvc@channelsvc.com",
                        Password = "I(0)1stog"
                    };

                    channelService.AuthHeaderValue = header;
                }

                try
                {
                    string message;
                    var doc = channelService.GetChannels(date, Request.QueryString["channel-id"], Request.QueryString["token"], Request.QueryString["length"], out message);


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