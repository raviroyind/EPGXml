using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using XmlParser.Core.DataContext;

namespace XmlParser.Secured
{
    public partial class ChannelSelection : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PopulateSources();
                PopulateInactive(Convert.ToInt64(ddlSourceXml.SelectedValue));
                PopulateActive(Convert.ToInt64(ddlSourceXml.SelectedValue));

                if (Convert.ToString(Session["USR_TYPE"]) == "Admin")
                {
                    hypHome.NavigateUrl = "../Admin/Dashboard.aspx";
                }

                lblUser.Text = "Welcome " + Convert.ToString(Session["USER_KEY"]);
            }
        }

        void PopulateInactive(long srno=0)
        {
            using (var dataContext = new EPGDataModel())
            {
                var dt = dataContext.ActiveChannels.Where(s => s.IsActive == false).ToList();

                if(srno!=0)
                    dt = dt.Where(s => s.SourceId.Equals(srno)).ToList();

                undo_redo.DataSource = dt;
                undo_redo.DataTextField = "ChannelName";
                undo_redo.DataValueField = "Srno";
                undo_redo.DataBind();
            }
        }

        void PopulateActive(long srno = 0)
        {
            using (var dataContext = new EPGDataModel())
            {
                var dt = dataContext.ActiveChannels.Where(s => s.IsActive).ToList();

                if (srno != 0)
                    dt = dt.Where(s => s.SourceId.Equals(srno)).ToList();

                undo_redo_to.DataSource = dt;
                undo_redo_to.DataTextField = "ChannelName";
                undo_redo_to.DataValueField = "Srno";
                undo_redo_to.DataBind();
            }
        }

        void PopulateSources()
        {
            using (var dataContext=new EPGDataModel())
            {
                ListItem[] sourceList = dataContext.SourceURLs.Select(ct => new ListItem { Value = ct.Srno.ToString(), Text = ct.Url }).ToArray();
                 
                foreach (var item in sourceList)
                {
                    item.Text = item.Text.Substring(item.Text.LastIndexOf("/", StringComparison.Ordinal) + 1);

                    if (item.Text.Contains("?"))
                        item.Text = item.Text.Substring(0, item.Text.LastIndexOf("?", StringComparison.Ordinal));
                }
                
                ddlSourceXml.Items.Clear();
                ddlSourceXml.Items.AddRange(sourceList);

                ddlSourceXml.ClearSelection();

                if (Request.QueryString["id"] != null)
                {
                    ddlSourceXml.Items.FindByValue(Request.QueryString["id"]).Selected = true;
                    ddlSourceXml.Enabled = false;
                }
                else
                    ddlSourceXml.SelectedIndex = 0;
            }
        }

        protected void OnClick(object sender, EventArgs e)
        {
            using (var dataContext = new EPGDataModel())
            {
                if (!string.IsNullOrEmpty(hidRight.Value))
                {
                    var activeIds = hidRight.Value.Substring(0, hidRight.Value.Length - 1).Split(',');

                    foreach (var item in activeIds)
                    {
                        var id = Convert.ToInt64(item);
                        var dbEntry = dataContext.ActiveChannels.FirstOrDefault(c => c.Srno.Equals(id));

                        if (dbEntry != null)
                        {
                            dbEntry.IsActive = true;
                            dataContext.Entry(dbEntry).State = EntityState.Modified;
                        }
                    }

                    dataContext.SaveChanges();
                    
                    PopulateInactive(Convert.ToInt64(ddlSourceXml.SelectedValue));
                    PopulateActive(Convert.ToInt64(ddlSourceXml.SelectedValue));
                    lblMsg.Text = "Selection Saved!";
                }

                if (!string.IsNullOrEmpty(hidLeft.Value))
                {
                    var inActiveIds = hidLeft.Value.Substring(0, hidLeft.Value.Length - 1).Split(',');
                     
                    foreach (var item in inActiveIds)
                    {
                        var id = Convert.ToInt64(item);
                        var dbEntry = dataContext.ActiveChannels.FirstOrDefault(c => c.Srno.Equals(id));

                        if (dbEntry != null)
                        {
                            dbEntry.IsActive = false;
                            dataContext.Entry(dbEntry).State = EntityState.Modified;
                        }
                    }

                    dataContext.SaveChanges();

                    PopulateInactive(Convert.ToInt64(ddlSourceXml.SelectedValue));
                    PopulateActive(Convert.ToInt64(ddlSourceXml.SelectedValue));
                    lblMsg.Text = "Selection Saved!";
                }

            }
        }

        protected void ddlSourceXml_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateInactive(Convert.ToInt64(ddlSourceXml.SelectedValue));
            PopulateActive(Convert.ToInt64(ddlSourceXml.SelectedValue));
        }
    }
}