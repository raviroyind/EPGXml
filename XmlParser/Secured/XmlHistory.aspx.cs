using BusinessLogic;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;
using Ionic.Zip;
using XmlParser.DataContext;

namespace XmlParser.Secured
{
    public partial class XmlHistory : CommonFunctions
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Hard Coding session keys to enable outsite access to this page.
            if (Session["USR_TYPE"] == null)
                Session.Add("USR_TYPE", "Admin");

            if (Session["USER_KEY"] == null)
                Session.Add("USER_KEY", "admin");
            //

            if (!IsPostBack)
            {
                ViewState["sortColumn"] = "ImportDate";
                ViewState["sortDirection"] = "DESC";

                //Start ################ Bellow code is for Initializing Paging ###############                
                TextBox txtPageno1 = (TextBox)ucPaging.FindControl("txtPageNo");
                
                txtPageno1.Attributes.Add("onkeypress", "return SetPagenoValue('" + txtPageno1.ClientID + "','" + txtPageno1.ClientID + "');");

                //End
                  

                if (Convert.ToString(Session["USR_TYPE"]) == "Admin")
                {
                    hypHomeLink.NavigateUrl = "../Admin/Dashboard.aspx";
                }

                lblUser.Text = "Welcome " + Convert.ToString(Session["USER_KEY"]);

                BindGrid();
            }
        }

        private void BindGrid()
        {
            var dataContext = new EPGDataModel();

            var searchList = dataContext.XmlImports.OrderByDescending(x => x.ImportDate).ToList();

            if (!string.IsNullOrEmpty(txtChannelName.Text))
                searchList = searchList.Where(c => c.XmlFileName.ToLower().Contains(txtChannelName.Text.ToLower())).ToList();

            if (!string.IsNullOrEmpty(txtImportDtFrom.Text))
            {
                var importDtFrom = Convert.ToDateTime(txtImportDtFrom.Text).Date;
                searchList = searchList.Where(c => c.ImportDate.Date >= importDtFrom).ToList();
            }

            if (!string.IsNullOrEmpty(txtImportDtTo.Text))
            {
                var importDtTo = Convert.ToDateTime(txtImportDtTo.Text).Date;
                searchList = searchList.Where(c => c.ImportDate.Date <= importDtTo).ToList();
            }
             
            var dtSearch = ToDataTable<XmlImport>(searchList);

            if (dtSearch!=null)
            {
                Session.Add("GetDataTable", dtSearch);
                ucPaging.BindPaging(gvFiles, dtSearch, ucPaging.PageNo, "txt",
                    Convert.ToString(ViewState["sortDirection"]), Convert.ToString(ViewState["sortColumn"]));
                BindBottomPaging(ucPaging, ucPaging1);
            }
        }

        /// <summary>
        /// This method return a datatable with empty row.
        /// </summary>
        /// <returns></returns>
        public DataTable ReturnEmptyDataTable()
        {
            var dtXml = new DataTable();

            var dcXmlFileName = new DataColumn("XmlFileName", typeof(System.String));
            dtXml.Columns.Add(dcXmlFileName);

            var dcEpgStartDt = new DataColumn("EpgStartDt", typeof(System.DateTime));
            dtXml.Columns.Add(dcEpgStartDt);

            var dcEpgEndDt = new DataColumn("EpgEndDt", typeof(System.DateTime));
            dtXml.Columns.Add(dcEpgEndDt);

            var dcImportDate = new DataColumn("ImportDate", typeof(System.DateTime));
            dtXml.Columns.Add(dcImportDate);

            var datatRow = dtXml.NewRow();

            dtXml.Rows.Add(datatRow);
            return dtXml;
        }

        protected void lnkDownload_OnClick(object sender, EventArgs e)
        {
            string filePath = (sender as LinkButton).CommandArgument;
            Response.ContentType = "application/xml";
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + Path.GetFileName(filePath));
            Response.WriteFile(filePath);
            Response.End();
        }

        protected void lnkBtnSearch_OnClick(object sender, EventArgs e)
        {
            BindGrid();
        }
         
        #region Paging and Sorting

        protected void ddlNoOfRecords_IndexChanged(object sender, EventArgs e)
        {

            ucPaging.PageNo = "1";

            DataTable dt = Session["FilterDataTable"] == null ? (DataTable)Session["GetDataTable"] : (DataTable)Session["FilterDataTable"];
            ucPaging.BindPaging(gvFiles, dt, ucPaging.PageNo, "First", Convert.ToString(ViewState["sortDirection"]), Convert.ToString(ViewState["sortColumn"]));
            BindBottomPaging(ucPaging, ucPaging1);
        }

        protected void txtPageNo_Changed(object sender, EventArgs e)
        {
            ucPaging.PageNo = (ucPaging.FindControl("txtPageNo") as TextBox).Text;
            DataTable dt = Session["FilterDataTable"] == null ? (DataTable)Session["GetDataTable"] : (DataTable)Session["FilterDataTable"];

            ucPaging.BindPaging(gvFiles, dt, ucPaging.PageNo, "txt", Convert.ToString(ViewState["sortDirection"]), Convert.ToString(ViewState["sortColumn"]));
            BindBottomPaging(ucPaging,ucPaging1);
        }

        protected void ImgbtnNavigator_Click(object sender, EventArgs e)
        {
            DataTable dt = Session["FilterDataTable"] == null ? (DataTable)Session["GetDataTable"] : (DataTable)Session["FilterDataTable"];
            ucPaging.BindPaging(gvFiles, dt, ucPaging.PageNo, ucPaging.NavType, Convert.ToString(ViewState["sortDirection"]), Convert.ToString(ViewState["sortColumn"]));
            BindBottomPaging(ucPaging, ucPaging1);
        }

        protected void txtPageNo1_Changed(object sender, EventArgs e)
        {
            ucPaging1.PageNo = (ucPaging.FindControl("txtPageNo") as TextBox).Text;
            DataTable dt = Session["FilterDataTable"] == null ? (DataTable)Session["GetDataTable"] : (DataTable)Session["FilterDataTable"];
            ucPaging1.BindPaging(gvFiles, dt, ucPaging1.PageNo, "txt", Convert.ToString(ViewState["sortDirection"]), Convert.ToString(ViewState["sortColumn"]));
            BindBottomPaging(ucPaging1, ucPaging);
        }

        protected void ImgbtnNavigator1_Click(object sender, EventArgs e)
        {
            DataTable dt = Session["FilterDataTable"] == null ? (DataTable)Session["GetDataTable"] : (DataTable)Session["FilterDataTable"];
            ucPaging1.BindPaging(gvFiles, dt, ucPaging1.PageNo, ucPaging1.NavType, Convert.ToString(ViewState["sortDirection"]), Convert.ToString(ViewState["sortColumn"]));
            BindBottomPaging(ucPaging1, ucPaging);
        }

        protected void gvFiles_Sorting(object sender, EventArgs e)
        {
            LinkButton lnkbtn = (LinkButton)sender;
            SetSorting(lnkbtn.CommandArgument);
            DataTable dt = Session["FilterDataTable"] == null ? (DataTable)Session["GetDataTable"] : (DataTable)Session["FilterDataTable"];
            ucPaging1.BindPaging(gvFiles, dt, ucPaging1.PageNo, ucPaging1.NavType, Convert.ToString(ViewState["sortDirection"]), Convert.ToString(ViewState["sortColumn"]));
            BindBottomPaging(ucPaging1, ucPaging);
        }

        protected void gvFilesImg_Sorting(object sender, EventArgs e)
        {

            ImageButton imgbtn = (ImageButton)sender;
            SetSorting(imgbtn.CommandArgument);
            DataTable dt = Session["FilterDataTable"] == null ? (DataTable)Session["GetDataTable"] : (DataTable)Session["FilterDataTable"];
            ucPaging1.BindPaging(gvFiles, dt, ucPaging1.PageNo, ucPaging1.NavType, Convert.ToString(ViewState["sortDirection"]), Convert.ToString(ViewState["sortColumn"]));
            BindBottomPaging(ucPaging1, ucPaging);
        }

        #endregion Paging and Sorting

        #region Filter

        protected void imgbtnRemoveFilter_OnClick(object sender, EventArgs e)
        {
            var lnkbtn = (LinkButton)sender;

            switch (lnkbtn.CommandArgument)
            {
                case "SourceURL":
                    hdnRptKeysFilter.Value = "";
                    (gvFiles.HeaderRow.FindControl("chklstRptKeys") as CheckBoxList).ClearSelection();
                    break;
            }

            FilterGridView();
        }

        private void FilterGridView()
        {
            DataTable dt = (DataTable)Session["GetDataTable"];

            IEnumerable<DataRow> dr = null;

            CheckBoxList cblstRptkeys = (CheckBoxList)gvFiles.HeaderRow.FindControl("chklstRptKeys");
            string s = "";

            foreach (ListItem item in cblstRptkeys.Items)
            {
                if (item.Selected)
                    s += item.Value + ":";
            }

            dr = from f in dt.AsEnumerable()
                 select f;

            if (s.Length > 0)
            {
                hdnRptKeysFilter.Value = s.Substring(0, s.Length - 1);
                string[] sRptkeys = s.Substring(0, s.Length - 1).Split(':');

                dr = dr.Where(c => sRptkeys.Contains(c.Field<string>("SourceURL")));
            }
            else
            {
                hdnRptKeysFilter.Value = "";
            }



            if (dr.Count() > 0)
            {
                Session.Add("FilterDataTable", dr.CopyToDataTable());
                ucPaging1.PageNo = "1";
                ucPaging1.BindFilter(gvFiles, (DataTable)Session["FilterDataTable"], ucPaging1.PageNo, Convert.ToString(ViewState["sortDirection"]), Convert.ToString(ViewState["sortColumn"]));
                BindBottomPaging(ucPaging1, ucPaging);
            }
            else
            {
                Session.Remove("FilterDataTable");
                BindGrid();
            }
        }

        #endregion Filter

        protected void imgbtnFilter_OnClick(object sender, EventArgs e)
        {
            FilterGridView();
        }
        protected void lnkBtnResetSearch_Click(object sender, EventArgs e)
        {
            txtChannelName.Text = string.Empty;
            txtImportDtFrom.Text=string.Empty;
            txtImportDtTo.Text = string.Empty;
            (gvFiles.HeaderRow.FindControl("chklstRptKeys") as CheckBoxList).ClearSelection();
            FilterGridView();
            BindGrid();
        }

        protected void gvFiles_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var hypView2 = (HyperLink)e.Row.FindControl("hypView2");
                if (string.IsNullOrEmpty(hypView2.NavigateUrl))
                    hypView2.Visible = false;
            }

            if (e.Row.RowType == DataControlRowType.Header)
            {
                HtmlGenericControl divFilter1 = (HtmlGenericControl)e.Row.FindControl("divProductNameFilter");
                (e.Row.FindControl("imgbtnFilter") as ImageButton).Attributes.Add("onclick", "javascript:ShowFilter('" + divFilter1.ClientID + "');return false;");
                (e.Row.FindControl("imgbtnCloseFilter") as ImageButton).Attributes.Add("onclick", "javascript:ShowFilter('" + divFilter1.ClientID + "');return false;");

                BindRptkeysFilter(e.Row.FindControl("chklstRptKeys") as CheckBoxList, e.Row);

            }
        }

        private void BindRptkeysFilter(CheckBoxList ddl, GridViewRow row)
        {

            if (Session["GetDataTable"] != null)
            {
                DataTable dtSessTbl = (DataTable)Session["GetDataTable"];
                DataTable dt = dtSessTbl.DefaultView.ToTable(true, new string[] { "SourceURL" });

                ddl.DataSource = dt;
                ddl.DataTextField = "SourceURL";
                ddl.DataValueField = "SourceURL";
                ddl.DataBind();
                //}
                
                ImageButton imgFilter1 = (ImageButton)row.FindControl("imgbtnFilter");

                imgFilter1.ImageUrl = hdnRptKeysFilter.Value != "" ? "../Images/filter_o.png" : "../Images/filter.png";

                if (hdnRptKeysFilter.Value != "")
                {
                    string[] s = hdnRptKeysFilter.Value.Split(':');

                    foreach (string str in s)
                    {
                        ddl.Items.FindByValue(str).Selected = true;
                    }
                }

                var divChklst = (HtmlGenericControl)row.FindControl("divChklstRpt");
                
                divChklst.Style.Add(HtmlTextWriterStyle.Height, "150px");
                divChklst.Style.Add(HtmlTextWriterStyle.OverflowY, "scroll");
               
            }
        }
 
        protected void lnkBtnDownload_OnClick(object sender, EventArgs e)
        {
            using (var zip = new ZipFile())
            {
                zip.AlternateEncodingUsage = ZipOption.AsNecessary;
                zip.AddDirectoryByName("EPGXmls");

                foreach (var dataKey in from GridViewRow row in gvFiles.Rows
                    where row.RowType == DataControlRowType.DataRow
                    where ((CheckBox) row.FindControl("chkSelect")).Checked
                    select Convert.ToString(gvFiles.DataKeys[row.RowIndex]["URL"])
                    into dataKey
                    where File.Exists(Server.MapPath(dataKey))
                    select dataKey)
                {
                    var filePath = Server.MapPath(dataKey);
                    zip.AddFile(filePath, "EPGXmls");
                }

                if (zip.Count <= 0) return;
                Response.Clear();
                Response.BufferOutput = false;
                var zipName = String.Format("EPGXmls_{0}.zip", DateTime.Now.ToString("yyyy-MMM-dd-HHmmss"));
                Response.ContentType = "application/zip";
                Response.AddHeader("content-disposition", "attachment; filename=" + zipName);
                zip.Save(Response.OutputStream);
                Response.End();
            }
        }
    }
}