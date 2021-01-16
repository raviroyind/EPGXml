using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using XmlParser.Core.DataContext;

namespace XmlParser.Code
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
         
        public static string GetIpAddress()
        {

            System.Web.HttpContext context = System.Web.HttpContext.Current;
            var ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                var addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    return addresses[0];
                }
            }

            return context.Request.ServerVariables["REMOTE_ADDR"];
        }

        /// <summary>
        /// Add Time Offset to Start & End attributes of output xml based on query string value for offset.
        /// </summary>
        /// <param name="originalTimeSpan">Original Start/ Stop value.</param>
        /// <param name="newOffset">Hours Offset to be added to original timespan.</param>
        /// <returns>String representation of original DateTime value after adding hours offset.</returns>
        public static string GetDateAddingOffset(string originalTimeSpan, int newOffset)
        {
            var dtAdditionalValue = string.Empty;
             

            if (originalTimeSpan.Contains("+"))
            {
                dtAdditionalValue = originalTimeSpan.Substring(originalTimeSpan.IndexOf('+'));
                originalTimeSpan = originalTimeSpan.Substring(0, originalTimeSpan.IndexOf('+')).Trim();
            }

            //Added on 5th-Jan-2016.
            //Handle single digit months by adding a leading zero. This is a bug in c# according to msdn while converting from UNIX format dates.
            if (originalTimeSpan.Length < 14)
            {
                var year = originalTimeSpan.Substring(0, 4);
                originalTimeSpan = year + "0" + originalTimeSpan.Substring(4);
            }

            var originalDate = DateTime.ParseExact(originalTimeSpan, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                                                                      
            var newDate = originalDate.AddHours(newOffset);

            var retval = newDate.Year.ToString() + newDate.Month.ToString() + newDate.Day.ToString("00").ToString() +
                           newDate.Hour.ToString("00") + newDate.Minute.ToString("00") + newDate.Second.ToString("00");

            return retval + " " + dtAdditionalValue;
        }

        public static bool SendMail(string toEmail, string subject, bool isBodyHtml, string body, bool enableSsl)
        {
            var bSuccess = true;
            var fromEmail = Convert.ToString(ConfigurationManager.AppSettings["SMPT_USER"]);
            var fromPassword = Convert.ToString(ConfigurationManager.AppSettings["SMPT_PASS"]);
            var msg = new MailMessage(fromEmail, toEmail, subject, body) {IsBodyHtml = true};
            try
            {
                var client = new SmtpClient(
                    Convert.ToString(ConfigurationManager.AppSettings["SMPT_SERVER"]), 
                    Convert.ToInt32(ConfigurationManager.AppSettings["SMPT_PORT"]))
                {
                    Credentials = new NetworkCredential(fromEmail, fromPassword),
                    EnableSsl = true
                };

                client.Send(msg);
            }
            catch (Exception ex)
            {
                bSuccess = false;
            }
            return bSuccess;
        }

        private const string Alg = "HmacSHA256";
        private const string Salt = "rz8LuOtFBXphj9WQfvFh";
  
        public static string GetHashedPassword(string password)
        {
            string key = string.Join(":", new string[] { password, Salt });

            using (HMAC hmac = HMACSHA256.Create(Alg))
            {
                // Hash the key.
                hmac.Key = Encoding.UTF8.GetBytes(Salt);
                hmac.ComputeHash(Encoding.UTF8.GetBytes(key));

                return Convert.ToBase64String(hmac.Hash);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public static string GetChannelId(string channel)
        {
            var returnId = string.Empty;
            using (var dataContext = new EPGDataModel())
            {
                var dbChannel = dataContext.XmlImports.FirstOrDefault(x => x.XmlFileName.Equals(channel));

                if (dbChannel == null)
                {
                    returnId = RandomString(5);
                }
                else if (!string.IsNullOrEmpty(dbChannel.ChannelId))
                {
                    returnId = dbChannel.ChannelId;
                }
                else
                {
                    returnId = RandomString(5);
                }
            }

            return returnId;
        }

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var rndString = new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray()).ToLower();

            using (var dataContext = new EPGDataModel())
            {
                var dbChannel = dataContext.XmlImports.FirstOrDefault(x => x.ChannelId.Equals(rndString));
                if (dbChannel != null)
                {
                    rndString = new string(Enumerable.Repeat(chars, length)
                        .Select(s => s[random.Next(s.Length)]).ToArray()).ToLower();
                }
            }

            return rndString;
        }


        public static XDocument GetPastDataXml(string channel, string channelId, string suffix = "")
        {
            #region Namespace...

            XNamespace xsd = "http://www.w3.org/2001/XMLSchema";
            XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
            XNamespace schemaLocation = XNamespace.Get("tv:noNamespaceSchemaLocation");
                
            #endregion Namespace...
            
            var doc = new XDocument();
            doc.Add(new XElement("tv",
                new XAttribute(XNamespace.Xmlns + "xsd", xsd),
                new XAttribute(XNamespace.Xmlns + "xsi", xsi)
                 ,new XAttribute(xsi + "schemaLocation", schemaLocation)));

            doc.Root.Add(new XElement("channel",
               new XAttribute("id", channelId),
               new XAttribute("name", channel),
                    new XElement("display-name", channel))
               );

             
            for (var x = 7; x >= 1; x--)
            {
                var xmlPath = ConfigurationManager.AppSettings["PastXmlFolderPath"] + "Day" + x + @"\"+ channel + suffix +".xml";

                if (!File.Exists(xmlPath))
                    continue;

                var xDoc = XDocument.Load(xmlPath);

                doc.Root.Add(xDoc.Descendants()
                       .DescendantNodes()
                       .OfType<XElement>()
                       .Where(n => n.Name.LocalName.Equals("programme")));
            }

            return doc;
           
        }
    }
}
