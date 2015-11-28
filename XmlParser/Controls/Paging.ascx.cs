using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace XmlParser.Controls
{
    public partial class Paging : System.Web.UI.UserControl
    {


        public event EventHandler NoOfRecords_SelectedIndexChanged;
        public event EventHandler PageNo_Changed;
        public event EventHandler Navigator_Click;

        private string sNavType = string.Empty;

        public bool ShowNoOfRecordsDropDown
        {
            set
            {
                tdNoOfRecords.Style[HtmlTextWriterStyle.Display] = "block";
                if (!Convert.ToBoolean(value))
                    tdNoOfRecords.Style[HtmlTextWriterStyle.Display] = "none";
            }
        }

        private int iPageSize = 50;
        public int PageSize
        {
            get
            {
                if (ddlNoOfRecords.SelectedValue == "")
                    return 50;
                else
                    return Convert.ToInt32(ddlNoOfRecords.SelectedValue);
                // return iPageSize;
            }
            set
            {
                iPageSize = Convert.ToInt32(value.ToString());
                ListItem item = ddlNoOfRecords.Items.FindByValue(value.ToString());
                if (item != null)
                    item.Selected = true;
                else
                    ddlNoOfRecords.Items.Add(new ListItem(value.ToString(), value.ToString()));
            }
        }

        public string PageNo
        {
            get
            {
                if (hdnTxtPageNo.Value == "")
                    return "1";
                else
                    return hdnTxtPageNo.Value;
            }
            set
            {
                hdnTxtPageNo.Value = value;
                txtPageNo.Text = hdnTxtPageNo.Value;
            }
        }

        public string SetPages
        {
            get
            {
                return lblTotPages.Text;
            }
            set
            {
                lblTotPages.Text = value;
            }
        }

        public string NavType
        {
            get { return sNavType; }
            set { sNavType = value; }
        }

        public string Align
        {
            set { tdNav.Attributes.Add("align", Convert.ToString(value)); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lnkimgbtnFirst.Enabled = false;
                lnkimgbtnPrevious.Enabled = false;
                txtPageNo.Style[HtmlTextWriterStyle.TextAlign] = "center";
                txtPageNo.Style[HtmlTextWriterStyle.VerticalAlign] = "center";
                btnPageNo.Style[HtmlTextWriterStyle.Display] = "none";
                BindNoOfRecords();
            }
        }

        private void BindNoOfRecords()
        {
            ddlNoOfRecords.Items.Clear();
            ddlNoOfRecords.Items.Add(new ListItem("10", "10"));
            ddlNoOfRecords.Items.Add(new ListItem("20", "20"));
            ddlNoOfRecords.Items.Add(new ListItem("50", "50"));
            ddlNoOfRecords.Items.Add(new ListItem("100", "100"));
            ddlNoOfRecords.Items.Add(new ListItem("150", "150"));
            ddlNoOfRecords.Items.Add(new ListItem("200", "200"));
            ddlNoOfRecords.Items.Add(new ListItem("250", "250"));

            if (ddlNoOfRecords.Items.Contains(new ListItem(iPageSize.ToString())))
                ddlNoOfRecords.Items.FindByValue(iPageSize.ToString()).Selected = true;
        }

        protected void ddlNoOfRecords_SelectedIndexChanged(object sender, EventArgs e)
        {
            PageNo = "1";
            PageSize = Convert.ToInt32(ddlNoOfRecords.SelectedValue);
            EnableNavigation(true);
            SelectedIndexChanged(e);

        }

        private void SelectedIndexChanged(EventArgs e)
        {
            if (NoOfRecords_SelectedIndexChanged != null)
            {
                NoOfRecords_SelectedIndexChanged(this, e);
            }
        }

        protected void btnPageNo_Click(object sender, EventArgs e)
        {
            PageNo = txtPageNo.Text;

            if (PageNo_Changed != null)
            {
                PageNo_Changed(sender, e);
            }
        }

        protected void imgbtn_Click(object sender, EventArgs e)
        {
            LinkButton imgbtn = (LinkButton)sender;
            NavType = imgbtn.CommandArgument;
            if (Navigator_Click != null)
            {
                Navigator_Click(sender, e);
            }
        }

        
        public void EnableNavigation(bool bIsEnable)
        {
            lnkimgbtnFirst.Enabled = bIsEnable;
            lnkimgbtnPrevious.Enabled = bIsEnable;
            lnkimgbtnNext.Enabled = bIsEnable;
            lnkimgbtnLast.Enabled = bIsEnable;
             
        }

        public void SetNavigation(string sNavType, string sPageNo, int iRecords, int iPageSize, int iRows, bool bIsTopPaging)
        {
            EnableNavigation(true);

            int iPageNo = sPageNo == "" ? 1 : Convert.ToInt32(sPageNo);

            int iPages = iRecords / iPageSize * iPageSize != iRecords ? iRecords / iPageSize + 1 : iRecords / iPageSize;

            lblTotPages.Text = "of " + iPages;

            if (iPages > 1)
            {
                switch (sNavType)
                {
                    case "First":
                        PageNo = "1";
                        lnkimgbtnFirst.Enabled = false;
                        lnkimgbtnPrevious.Enabled = false;
                        break;
                    case "Previous":
                        PageNo = (Convert.ToInt32(iPageNo) - 1).ToString();
                        if (Convert.ToInt32(PageNo) <= 1)
                        {
                            lnkimgbtnFirst.Enabled = false;
                            lnkimgbtnPrevious.Enabled = false;
                        }
                        break;
                    case "Next":
                        PageNo = (Convert.ToInt32(iPageNo) + 1).ToString();
                        if (Convert.ToInt32(PageNo) >= Convert.ToInt32(iPages))
                        {
                            lnkimgbtnNext.Enabled = false;
                            lnkimgbtnLast.Enabled = false;
                        }
                        break;
                    case "Last":
                        PageNo = Convert.ToString(iPages);
                        lnkimgbtnNext.Enabled = false;
                        lnkimgbtnLast.Enabled = false;
                        break;
                    default:
                        PageNo = Convert.ToString(iPageNo);
                        if (Convert.ToInt32(PageNo) <= 1)
                        {
                            lnkimgbtnFirst.Enabled = false;
                            lnkimgbtnPrevious.Enabled = false;
                        }
                        else if (Convert.ToInt32(PageNo) >= Convert.ToInt32(iPages))
                        {
                            lnkimgbtnNext.Enabled = false;
                            lnkimgbtnLast.Enabled = false;
                        }
                        break;
                }
            }
            else
            {
                EnableNavigation(false);
            }
            txtPageNo.Text = PageNo;
           
        }

        public void BindPaging(GridView gv, DataTable dt, string sPageNo, string sNavType, string sSortDirection, string sSortColumn, ref DataTable dtSearch)
        {

            if (dt != null)
            {
                int iRecords = dt.Rows.Count;

                SetNavigation(sNavType, sPageNo, iRecords, Convert.ToInt32(PageSize), gv.Rows.Count, true);

                BindGrid(gv, dt, Convert.ToInt32(PageSize), sSortColumn, sSortDirection, ref dtSearch);

                SetPageRecordsData(PageNo, PageSize, iRecords, gv.Rows.Count);

                SetSortedImage(gv, sSortDirection, sSortColumn);

            }
        }

        public void BindPaging(GridView gv, DataTable dt, string sPageNo, string sNavType, string sSortDirection, string sSortColumn)
        {

            if (dt != null)
            {
                int iRecords = dt.Rows.Count;

                SetNavigation(sNavType, sPageNo, iRecords, Convert.ToInt32(PageSize), gv.Rows.Count, true);

                BindGrid(gv, dt, Convert.ToInt32(PageSize), sSortColumn, sSortDirection);

                SetPageRecordsData(PageNo, PageSize, iRecords, gv.Rows.Count);

                SetSortedImage(gv, sSortDirection, sSortColumn);

            }
        }

        private void BindGrid(GridView gv, DataTable dt, int iRecPerPage, string sSortColumn, string sSortDirection)
        {
            DataTable dTable = new DataTable();
            bool isEmpty = false;
            if (dt.Rows.Count > 0)
                dTable = GetDataTableBySortAndPaging(dt, iRecPerPage, sSortDirection, sSortColumn);
            else
            {
                dTable = GetHeader(dt);
                isEmpty = true;
                PageNo = "0";
            }

            gv.DataSource = dTable;
            gv.DataBind();

            if (isEmpty)
                gv.Rows[0].Visible = false;

        }

        private void BindGrid(GridView gv, DataTable dt, int iRecPerPage, string sSortColumn, string sSortDirection, ref DataTable dtSearch)
        {
            DataTable dTable = new DataTable();
            bool isEmpty = false;
            if (dt.Rows.Count > 0)
                dTable = GetDataTableBySortAndPaging(dt, iRecPerPage, sSortDirection, sSortColumn);
            else
            {
                dTable = GetHeader(dt);
                isEmpty = true;
                PageNo = "0";
            }
            dtSearch = dTable;
            gv.DataSource = dTable;
            gv.DataBind();

            if (isEmpty)
                gv.Rows[0].Visible = false;

        }

        public void BindGridView(GridView gv, DataTable dt, string sPageNo, string sSortColumn, string sSortDirection, ref DataTable dtSearch)
        {

            int iRecords = dt.Rows.Count;
            //ViewState["DataTable"] = dt;

            BindGrid(gv, dt, Convert.ToInt32(PageSize), sSortColumn, sSortDirection, ref dtSearch);

            if (Convert.ToInt32(PageNo) <= 1)
                SetNavigation("First", sPageNo, iRecords, Convert.ToInt32(PageSize), gv.Rows.Count, true);

            SetSortedImage(gv, sSortDirection, sSortColumn);

            SetPageRecordsData(PageNo, PageSize, iRecords, gv.Rows.Count);

            if (gv.Rows.Count <= 0)
                ddlNoOfRecords.Enabled = false;

        }

        public void BindGridView(GridView gv, DataTable dt, string sPageNo, string sSortColumn, string sSortDirection)
        {

            int iRecords = dt.Rows.Count;

            BindGrid(gv, dt, Convert.ToInt32(PageSize), sSortColumn, sSortDirection);

            if (Convert.ToInt32(PageNo) <= 1)
                SetNavigation("First", sPageNo, iRecords, Convert.ToInt32(PageSize), gv.Rows.Count, true);


            SetPageRecordsData(PageNo, PageSize, iRecords, gv.Rows.Count);

            SetSortedImage(gv, sSortDirection, sSortColumn);

            if (gv.Rows.Count <= 0)
                ddlNoOfRecords.Enabled = false;
        }

        private void SetPageRecordsData(string sPageNo, int iPageSize, int iTotRecords, int iPerPageCount)
        {

            lblRecords.Text = "";

            if (iTotRecords > 0)
            {
                int i = (((Convert.ToInt32(sPageNo) - 1) * iPageSize) + 1);
                lblRecords.Text = "Records <strong>" + i + " - " + ((i + iPerPageCount) - 1) + "</strong> of <strong>" + iTotRecords + "</strong>";
                ddlNoOfRecords.Enabled = true;
            }
            else
            {
                lblRecords.Text = "Records <strong>" + 0 + " - " + 0 + "</strong> of <strong> " + iTotRecords + "</strong>";
                ddlNoOfRecords.Enabled = false;
            }
        }

        private DataTable GetHeader(DataTable dt)
        {
            DataRow dr = dt.NewRow();

            dt.Rows.Add(dr);
            dt.AcceptChanges();

            return dt;
        }

        private void SetSortedImage(GridView gv, string sSortDirection, string sSortColumn)
        {
            if (sSortDirection != "" && sSortColumn != "")
            {
                ImageButton imgbtn = (ImageButton)gv.HeaderRow.FindControl("btnSort_" + sSortColumn);
                if (imgbtn != null)
                {
                    imgbtn.Visible = true;

                    if ("ASC" == sSortDirection.ToUpper())
                    {
                        imgbtn.ImageUrl = "../Images/but/sort03_asc.gif";
                        imgbtn.ToolTip = "Ascending";
                    }
                    else
                    {
                        imgbtn.ImageUrl = "../Images/but/sort03_desc.gif";
                        imgbtn.ToolTip = "Descending";
                    }
                }
            }
        }

        private DataTable GetDataTableBySortAndPaging(DataTable dt, int iRecordsPerPage, string sSortDirection, string sSortColumn)
        {
            IEnumerable<DataRow> v = null;
            if (sSortDirection.ToUpper() == "ASC")
            {
                v = (from order in dt.AsEnumerable()
                     orderby order.Field<object>(sSortColumn) ascending
                     select order).Skip((Convert.ToInt32(PageNo) - 1) * iRecordsPerPage).Take(iRecordsPerPage);
            }
            else if (sSortDirection.ToUpper() == "DESC")
            {
                v = (from order in dt.AsEnumerable()
                     orderby order.Field<object>(sSortColumn) descending
                     select order).Skip((Convert.ToInt32(PageNo) - 1) * iRecordsPerPage).Take(iRecordsPerPage);
            }
            else
            {
                v = (from order in dt.AsEnumerable()
                     select order).Skip((Convert.ToInt32(PageNo) - 1) * iRecordsPerPage).Take(iRecordsPerPage);
            }

            return v.CopyToDataTable();

        }

        public void BindFilter(GridView gv, DataTable dtSource, string sPageNo, string sSortDirection, string sSortColumn, ref DataTable dt)
        {
            BindPaging(gv, dtSource, "1", "First", sSortDirection, sSortColumn, ref dt);
        }

        public void BindFilter(GridView gv, DataTable dtSource, string sPageNo, string sSortDirection, string sSortColumn)
        {
            BindPaging(gv, dtSource, "1", "First", sSortDirection, sSortColumn);
        }

        #region Multi Sortable GridView

        public void BindMultiSortGridView(GridView gv, DataTable dt, string sPageNo, string sSortColumn, string sSortColumn2, string sSortDirection)
        {

            int iRecords = dt.Rows.Count;

            BindMultiSortGrid(gv, dt, Convert.ToInt32(PageSize), sSortColumn, sSortColumn2, sSortDirection);

            if (Convert.ToInt32(PageNo) <= 1)
                SetNavigation("First", sPageNo, iRecords, Convert.ToInt32(PageSize), gv.Rows.Count, true);


            SetPageRecordsData(PageNo, PageSize, iRecords, gv.Rows.Count);

            SetMultiSortedImage(gv, sSortDirection, sSortColumn, sSortColumn2);

            if (gv.Rows.Count <= 0)
                ddlNoOfRecords.Enabled = false;
        }

        private void BindMultiSortGrid(GridView gv, DataTable dt, int iRecPerPage, string sSortColumn, string sSortColumn2, string sSortDirection)
        {
            DataTable dTable = new DataTable();
            bool isEmpty = false;
            if (dt.Rows.Count > 0)
                dTable = GetDataTableBySortAndPaging(dt, iRecPerPage, sSortDirection, sSortColumn2, sSortColumn);
            else
            {
                dTable = GetHeader(dt);
                isEmpty = true;
                PageNo = "0";
            }

            gv.DataSource = dTable;
            gv.DataBind();

            if (isEmpty)
                gv.Rows[0].Visible = false;

        }

        private DataTable GetDataTableBySortAndPaging(DataTable dt, int iRecordsPerPage, string sSortDirection, string sSortColumn2, string sSortColumn)
        {
            IEnumerable<DataRow> v = null;
            if (sSortColumn == sSortColumn2)
            {
                if (sSortDirection.ToUpper() == "ASC")
                {
                    v = (from order in dt.AsEnumerable()
                         orderby order.Field<object>(sSortColumn) ascending
                         select order).Skip((Convert.ToInt32(PageNo) - 1) * iRecordsPerPage).Take(iRecordsPerPage);
                }
                else if (sSortDirection.ToUpper() == "DESC")
                {
                    v = (from order in dt.AsEnumerable()
                         orderby order.Field<object>(sSortColumn) descending
                         select order).Skip((Convert.ToInt32(PageNo) - 1) * iRecordsPerPage).Take(iRecordsPerPage);
                }
                else
                {
                    v = (from order in dt.AsEnumerable()
                         select order).Skip((Convert.ToInt32(PageNo) - 1) * iRecordsPerPage).Take(iRecordsPerPage);
                }
            }
            else
            {
                if (sSortDirection.ToUpper() == "ASC")
                {
                    v = (from order in dt.AsEnumerable()
                         orderby order.Field<object>(sSortColumn) ascending, order.Field<object>(sSortColumn2) ascending
                         select order).Skip((Convert.ToInt32(PageNo) - 1) * iRecordsPerPage).Take(iRecordsPerPage);
                }
                else if (sSortDirection.ToUpper() == "DESC")
                {
                    v = (from order in dt.AsEnumerable()
                         orderby order.Field<object>(sSortColumn) descending, order.Field<object>(sSortColumn2) ascending
                         select order).Skip((Convert.ToInt32(PageNo) - 1) * iRecordsPerPage).Take(iRecordsPerPage);
                }
                else
                {
                    v = (from order in dt.AsEnumerable()
                         select order).Skip((Convert.ToInt32(PageNo) - 1) * iRecordsPerPage).Take(iRecordsPerPage);
                }
            }
            return v.CopyToDataTable();

        }

        private void SetMultiSortedImage(GridView gv, string sSortDirection, string sSortColumn, string sSortColumn2)
        {

            if (sSortDirection != "" && sSortColumn != "")
            {

                ImageButton imgbtn = (ImageButton)gv.HeaderRow.FindControl("btnSort_" + sSortColumn);

                if (imgbtn != null)
                {
                    imgbtn.Visible = true;

                    if ("ASC" == sSortDirection.ToUpper())
                    {
                        imgbtn.ImageUrl = "../Images/but/sort03_asc.gif";
                        imgbtn.ToolTip = "Ascending";
                    }
                    else
                    {
                        imgbtn.ImageUrl = "../Images/but/sort03_desc.gif";
                        imgbtn.ToolTip = "Descending";
                    }
                }
                if (sSortColumn != sSortColumn2)
                {
                    ImageButton imgbtn2 = (ImageButton)gv.HeaderRow.FindControl("btnSort_" + sSortColumn2);

                    if (imgbtn2 != null)
                    {
                        imgbtn2.Visible = true;

                        imgbtn2.ImageUrl = "../Images/but/sort03_asc.gif";
                        imgbtn2.ToolTip = "Ascending";

                    }
                }
            }
        }

        public void BindMultiSortPaging(GridView gv, DataTable dt, string sPageNo, string sNavType, string sSortDirection, string sSortColumn, string sSortColumn2)
        {

            if (dt != null)
            {
                int iRecords = dt.Rows.Count;

                SetNavigation(sNavType, sPageNo, iRecords, Convert.ToInt32(PageSize), gv.Rows.Count, true);

                BindMultiSortGrid(gv, dt, Convert.ToInt32(PageSize), sSortColumn, sSortColumn2, sSortDirection);

                SetPageRecordsData(PageNo, PageSize, iRecords, gv.Rows.Count);

                SetMultiSortedImage(gv, sSortDirection, sSortColumn, sSortColumn2);

            }
        }

        #endregion Multi Sortable GridView
    }
}