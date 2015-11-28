using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using XmlParser.DataContext;

namespace XmlParser
{
    public partial class _default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Form.DefaultButton = this.btnLogin.UniqueID;

            if (Page.IsPostBack) return;
           
            Session.Abandon();
            Session.Clear();

            if (!Page.IsPostBack)
            {
                if (Request.QueryString["id"] != null)
                {
                    switch (Convert.ToString(Request.QueryString["id"]))
                    {
                        case "ua":
                            lblMsg.Text = "You are not authorized to use application!";
                            break;
                        case "lg":
                            lblMsg.Text = "You have sucessfully logged out!";
                            break;
                    }
                }
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            var userId = txtUserId.Text;
            var pass = txtPassword.Text;

            var dataModel=new EPGDataModel();
            var dbUser = dataModel.Users.Find(userId);

            if (dbUser != null)
            {
                if (dbUser.Password.Equals(pass))
                {
                    Session.Add("USER_KEY", userId);
                    Session.Add("USR_TYPE",dbUser.UserType.Trim());

                    if (dbUser.UserType.Trim().Equals("Admin"))
                        Response.Redirect("~/Admin/Dashboard.aspx");
                    else if (dbUser.UserType.Trim().Equals("User"))
                        Response.Redirect("~/Secured/index.aspx");
                }
                else
                    divMsg.Style.Add(HtmlTextWriterStyle.Display, "block");
            }
            else
                divMsg.Style.Add(HtmlTextWriterStyle.Display, "block");
        }
    }
}