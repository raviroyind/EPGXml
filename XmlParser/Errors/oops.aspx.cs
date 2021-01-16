using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using XmlParser.Code;

namespace XmlParser.Errors
{
    public partial class oops : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
             
            if (Session["EXP"] != null)
            {
                var exp = (Exception) Session["EXP"];
                var url = Convert.ToString(Session["URL"]);
                SendErrorMail(exp,url);
                errorMsg.InnerHtml = "Target Url: " + url +"<br/>";
                errorMsg.InnerHtml += "Error Message: " + exp.Message;
            }
        }
 
        private static void SendErrorMail(Exception exp, string url)
        {
            var mailMessage = new StringBuilder();
            mailMessage.Append("<br/><strong>Faulting Url: </strong>" + url + "<br/>");
            mailMessage.Append("<strong>Original Error: "+exp.Message +"<br/>");
               
            CommonFunctions.SendMail(ConfigurationManager.AppSettings["ERR_MAIL_TO"],
                ConfigurationManager.AppSettings["ERR_MAIL_SUB"], true, mailMessage.ToString(), true);

            if (HttpContext.Current.Session["EXP"] != null)
                HttpContext.Current.Session.Remove("EXP");

        }
    }
}