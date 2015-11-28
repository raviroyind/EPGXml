using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using XmlParser.DataContext;

namespace XmlParser.Admin
{
    public partial class Dashboard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["USER_KEY"] == null)
                Response.Redirect("../default.aspx?id=ua");
            else if (Convert.ToString(Session["USR_TYPE"])!="Admin")
                Response.Redirect("../default.aspx?id=ua");
            else 
                lblUser.Text = "Welcome " + Convert.ToString(Session["USER_KEY"]);

            if (!IsPostBack)
            {
                BindGrid();
            }
        }

        private void BindGrid()
        {
            using (var dataContext = new EPGDataModel())
            {
                var currentUser = Convert.ToString(Session["USER_KEY"]).Trim().ToLower();
                gvUsers.DataSource = dataContext.Users.Where(u => u.UserName.Trim().ToLower() != currentUser).ToList();
                gvUsers.DataBind();
            }
        }

        protected void Insert(object sender, EventArgs e)
        {
            using (var dataContext = new EPGDataModel())
            {
                var dbEntry = dataContext.Users.Where(u => u.UserName.ToLower().Equals(txtName.Text.ToLower())).FirstOrDefault();

                if (dbEntry != null)
                {
                    lblMsg.Text = "User " + txtName.Text + " already exsists!";
                    return;
                }

                var user = new User
                {
                    UserName=txtName.Text,
                    UserEmailAddress=txtUserEmailAddress.Text,
                    Password = txtPassword.Text,
                    UserType= ddlUserType.SelectedValue,
                    IsActive=true,
                    DateCreated=DateTime.Today,
                };
                dataContext.Users.Add(user);
                dataContext.SaveChanges();
            }

            BindGrid();
            lblMsg.Text = "User " + txtName.Text + " added successfully!";
            ClearForm();

        }

        private  void ClearForm()
        {
            txtName.Text = string.Empty;
            txtUserEmailAddress.Text = string.Empty;
            txtPassword.Text = string.Empty;
            ddlUserType.ClearSelection();
            ddlUserType.SelectedIndex = 0;
        }
        protected void gvUsers_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow && e.Row.RowIndex != gvUsers.EditIndex)
            {
                (e.Row.Cells[4].Controls[2] as ImageButton).Attributes["onclick"] =
                    "if(!confirm('Do you want to delete the record?')){ return false; };";
            }
        }

        protected void gvUsers_OnRowEditing(object sender, GridViewEditEventArgs e)
        {
            gvUsers.EditIndex = e.NewEditIndex;
            BindGrid();
           
        }

        protected void gvUsers_OnRowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvUsers.EditIndex = -1;
            BindGrid();
        }

        protected void gvUsers_OnRowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            var row = gvUsers.Rows[e.RowIndex];
            var userId = Convert.ToString(gvUsers.DataKeys[e.RowIndex].Values[0]);
            var userName = (row.FindControl("txtName") as TextBox).Text;
            var email = (row.FindControl("txtUserEmailAddress") as TextBox).Text;
            var pass = (row.FindControl("txtPassword") as TextBox).Text;
            var type = (row.FindControl("ddlUserType") as DropDownList).SelectedValue;

            using (var dataContext = new EPGDataModel())
            {
                var user = dataContext.Users.Find(userName);
                
                user.UserEmailAddress = email;
                user.Password = pass;
                user.PasswordSalt = pass;
                user.UserType = type;
                try
                {
                    dataContext.Entry(user).State = EntityState.Modified;
                    dataContext.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var eve in ex.EntityValidationErrors)
                    {
                        Console.WriteLine(
                            "Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                    throw;
                }

               
            }
            gvUsers.EditIndex = -1;
            BindGrid();
        }

        protected void gvUsers_OnRowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var userId = Convert.ToString(gvUsers.DataKeys[e.RowIndex].Values[0]);
            using (var dataContext = new EPGDataModel())
            {
                var user = dataContext.Users.Find(userId);
                dataContext.Users.Remove(user);
                dataContext.SaveChanges();
            }
            BindGrid();
        }

        
    }
}
