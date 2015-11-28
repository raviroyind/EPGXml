using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BusinessLogic
{
    public class CommonFunctions : Page
    {
        
        protected void BindBottomPaging(UserControl ucPaging, UserControl ucPaging1)
        {
            (ucPaging1.FindControl("txtPageNo") as TextBox).Text = (ucPaging.FindControl("txtPageNo") as TextBox).Text; 
            (ucPaging1.FindControl("lblTotPages") as Label).Text = (ucPaging.FindControl("lblTotPages") as Label).Text;
            (ucPaging1.FindControl("lnkimgbtnFirst") as LinkButton).Enabled = (ucPaging.FindControl("lnkimgbtnFirst") as LinkButton).Enabled;
            (ucPaging1.FindControl("lnkimgbtnPrevious") as LinkButton).Enabled = (ucPaging.FindControl("lnkimgbtnPrevious") as LinkButton).Enabled;
            (ucPaging1.FindControl("lnkimgbtnNext") as LinkButton).Enabled = (ucPaging.FindControl("lnkimgbtnNext") as LinkButton).Enabled;
            (ucPaging1.FindControl("lnkimgbtnLast") as LinkButton).Enabled = (ucPaging.FindControl("lnkimgbtnLast") as LinkButton).Enabled;
        }

        protected void SetSorting(string sSortExp)
        {
            if (Convert.ToString(ViewState["sortColumn"]) == sSortExp)
            {
                if (ViewState["sortDirection"] != null)
                {
                    if ("ASC" == ViewState["sortDirection"].ToString())
                        ViewState["sortDirection"] = "DESC";
                    else
                        ViewState["sortDirection"] = "ASC";
                }
                else
                    ViewState["sortDirection"] = "ASC";
            }
            else
            {
                ViewState["sortColumn"] = sSortExp;
                ViewState["sortDirection"] = "ASC";
            }

        }

        protected void BindDDL(DropDownList ddl, object src, string valueField, string textField)
        {
            
            ddl.DataSource = src;
            ddl.DataTextField = textField;
            ddl.DataValueField = valueField;
            ddl.DataBind();
            ddl.Items.Insert(0,new ListItem("--SELECT--","SEL"));
        }

        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }

            return dataTable;
        }
    }
}
