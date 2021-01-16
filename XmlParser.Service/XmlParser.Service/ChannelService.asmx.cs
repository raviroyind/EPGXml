using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Authentication;
using System.Text;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.Linq;
using XmlParser.Core;
using XmlParser.Core.DataContext;

namespace XmlParser.Service
{
    public class AuthHeader : SoapHeader
    {
        public string Username;
        public string Password;
    }

    /// <summary>
    /// Summary description for ChannelService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    [ScriptService]

    public class ChannelService : WebService
    {

        public AuthHeader Authentication;
        public static List<string> ExceptionsList;

        /// <summary>
        /// EPG Xml Service.
        /// </summary>
        /// <param name="epgStartDate">Date for which programmes should be fetched in xml.</param>
        /// <param name="channelIds">Comma Separated channel Ids or channel id, with or without offset "channelid[offset value]."</param>
        /// <param name="message">Out message returned by service. "200/OK" if successfull.</param>
        /// <param name="token">Authentication token used to access service in case of a Uri request.</param>
        /// <param name="lenght">lenght in minutes or seconds.</param>
        /// <returns>Single XmlDocument containing the information requested.</returns>
        [ScriptMethod(UseHttpGet = true)]
        [WebMethod]
        [SoapHeader("Authentication", Required = true)]
        public XmlDocument GetChannels(DateTime epgStartDate, string channelIds, out string message, string token = null,string lenght=null)
        {
            ExceptionsList=new List<string>();
            var isTokenRequest = false;
            var tokenAuthenticated = false;
            var authorization = false;
            message = "Unauthorized access is not allowed.";


            var finalDocument = new XDocument();
           
            if (!string.IsNullOrEmpty(token))
                isTokenRequest = true;

            if (Authentication == null && !isTokenRequest)
            {
                message = "Unauthorized access is not allowed.";
                throw new AuthenticationException("Unauthorized access is not allowed.");
            }
            else
            {
                if (isTokenRequest)
                {
                    using (var context = new EPGDataModel())
                    {
                        var auth =
                            context.Users.FirstOrDefault(u => u.AuthenticationToken.ToLower().Equals(token.ToLower()));

                        if (auth != null)
                        {
                            authorization = true;
                            tokenAuthenticated = true;
                        }
                        else
                        {
                            message = "Invalid Authorization Token";
                        }
                          
                    }
                }
                else if(Authentication.Username.Equals(ConfigurationManager.AppSettings["SVC_USR"]) && Authentication.Password.Equals(ConfigurationManager.AppSettings["SVC_PWD"]))
                {
                    authorization = true;
                }

                if (!isTokenRequest && Authentication!=null)
                {
                    if (string.IsNullOrEmpty(Authentication.Username) || string.IsNullOrEmpty(Authentication.Password))
                    {
                        message = "Unauthorized access is not allowed.";
                        throw new AuthenticationException("Unauthorized access is not allowed.");
                    }
                }
                
                try
                {
                         
                    if (authorization)
                    {
                        var commaSeparatedChannels = string.Empty;
                        var hashSet = new HashSet<string>();

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

                        finalDocument.Add(new XElement("tv", ""));

                        foreach (var item in GetChannels(epgStartDate, hashSet, lenght))
                        {
                            finalDocument.Root.Add(item.Nodes());
                        }

                        message = "200/OK";
                    }
                }
                catch (Exception exception)
                {
                    message = exception.Message;
                }


                if (ExceptionsList.Count > 0)
                {
                    var mailMessage = new StringBuilder();
                    mailMessage.Append("<br/><strong>Web Service call failed for following"+ (ExceptionsList.Count>1?" channels":" channel") +" : </strong><br/><br/>");
                    var iCount = 1;
                    foreach (var channel in ExceptionsList)
                    {
                        mailMessage.Append(iCount +".> "+ channel + " returned a blank document. (Date=" + epgStartDate.ToString("MM-dd-yyyy") + ")<br/><br/>");
                        iCount++;
                    }

                    mailMessage.Append("<br/><br/><strong>Administrator</strong><br/>");
                    mailMessage.Append("http://admin.latvf.net/");

                    SendMail(ConfigurationManager.AppSettings["ERR_MAIL_TO"],
                        "[Web Service Call] Channel error", true, mailMessage.ToString(), true);
                }

                return ToXmlDocument(finalDocument);
                 
            }
             
        }

        private static IEnumerable<XDocument> GetChannels(DateTime epgStartDate, HashSet<string> hashSet,string lenght)
        {
            return hashSet.Select(item => GetChannelTable(epgStartDate, item, lenght)).ToList();
        }

        private static XDocument GetChannelTable(DateTime epgStartDate, string channelIdwithOffset,string length)
        { 
            var filterList = new List<XElement>();
             
            var year = epgStartDate.Year.ToString();
            var month = epgStartDate.Month.ToString();
            var doubleDigitMonth = month.Length == 2;
            var day = epgStartDate.Day.ToString();
            if (day.Length == 1) 
                day = "0" + day;

            var unixTimeStamp = year + (doubleDigitMonth? month:"0"+ month) + day;
            var channel = string.Empty;
            string channelIdwithoutOffset;
            var offSet = 0;
            var isCustomChannel = false;
            var isPastData = false || epgStartDate.Date < DateTime.Today.Date;
            const string dayFolder = @"Past7\"; 
 

            if(channelIdwithOffset.Contains("["))
            {
                channelIdwithoutOffset = channelIdwithOffset.Substring(0, channelIdwithOffset.LastIndexOf("[", StringComparison.Ordinal));

                var start = channelIdwithOffset.IndexOf("[", StringComparison.Ordinal) + 1;
                var end = channelIdwithOffset.IndexOf("]", start, StringComparison.Ordinal);
                var result = channelIdwithOffset.Substring(start, end - start);

                offSet = Convert.ToInt32(result);
            }
            else
            {
                channelIdwithoutOffset = channelIdwithOffset;
            }

            //Month will always be double digit since we are adding leading "0" from 6th-April-2016
            doubleDigitMonth = true;


            using (var context = new EPGDataModel())
            {
                var channelName = context.XmlImports.FirstOrDefault(x => x.ChannelId.Contains(channelIdwithoutOffset));

                if (channelName != null)
                {
                    if (channelName.SourceUrl.Equals("Custom Channel"))
                    {
                        doubleDigitMonth = true;
                        unixTimeStamp = year + "0" + month + day;
                        isCustomChannel = true;
                    }

                    channel = Path.GetFileNameWithoutExtension(channelName.XmlFileName);

                    try
                    {
                        //var document =
                        //XDocument.Load(!isPastData ? ConfigurationManager.AppSettings["XML_PATH"] + channelName.XmlFileName :
                        //ConfigurationManager.AppSettings["PastXmlFolderPath"] + dayFolder + channelName.XmlFileName
                        //);

                        var document = !isPastData ? XDocument.Load( ConfigurationManager.AppSettings["XML_PATH"] + channelName.XmlFileName) : GetPastChannelTable(epgStartDate,channelIdwithOffset,length);

                        if (!isPastData)
                        {
                            filterList = document.Descendants()
                                .DescendantNodes()
                                .OfType<XElement>()
                                .Where(x => x.Name.LocalName.Equals("programme"))
                                .Where(x => x.Attribute("start").Value.Substring(0, doubleDigitMonth ? 8 : 7).Equals(unixTimeStamp)).ToList();
                        }
                        else
                        {
                            filterList = document.Descendants()
                                .DescendantNodes()
                                .OfType<XElement>()
                                .Where(x => x.Name.LocalName.Equals("programme")).ToList();
                        }
                    }
                    catch (FileNotFoundException feException)
                    {

                        if (!ExceptionsList.Contains(Path.GetFileNameWithoutExtension(channelName.XmlFileName)))
                        {
                            ExceptionsList.Add(Path.GetFileNameWithoutExtension(channelName.XmlFileName));
                        }
                         
                        return GetBlankXml(channelName);
                    }
                    
                }
            }

            if(!isPastData)
                return GetChannelXml(filterList, channelIdwithoutOffset, channel, offSet, isCustomChannel, length);
            else
                return GetPastChannelXml(filterList, channelIdwithoutOffset, channel, offSet, isCustomChannel, length);
        }

        private static XDocument GetPastChannelTable(DateTime epgStartDate, string channelIdwithOffset, string lenght)
        {
            var filterList = new List<XElement>();

            var year = epgStartDate.Year.ToString();
            var month = epgStartDate.Month.ToString();
            var doubleDigitMonth = month.Length == 2;
            var day = epgStartDate.Day.ToString();
            if (day.Length == 1)
                day = "0" + day;

            //Task ID:  1833 - previous days data missing
            //var unixTimeStamp = year + (doubleDigitMonth ? "0" + month : month) + day;
            var unixTimeStamp = year + (doubleDigitMonth ? month : "0" + month) + day;
            var channel = string.Empty;
            var channelIdwithoutOffset = string.Empty;
            var offSet = 0;
            var IsCustomChannel = false;

            if (channelIdwithOffset.Contains("["))
            {
                channelIdwithoutOffset = channelIdwithOffset.Substring(0, channelIdwithOffset.LastIndexOf("[", StringComparison.Ordinal));

                var start = channelIdwithOffset.IndexOf("[", StringComparison.Ordinal) + 1;
                var end = channelIdwithOffset.IndexOf("]", start, StringComparison.Ordinal);
                var result = channelIdwithOffset.Substring(start, end - start);

                offSet = Convert.ToInt32(result);
            }
            else
            {
                channelIdwithoutOffset = channelIdwithOffset;
            }

            //Month will always be double digit since we are adding leading "0" from 6th-April-2016
            doubleDigitMonth = true;
            using (var context = new EPGDataModel())
            {
                var channelName = context.XmlImports.FirstOrDefault(x => x.ChannelId.Contains(channelIdwithoutOffset));

                if (channelName != null)
                {
                    if (channelName.SourceUrl.Equals("Custom Channel"))
                    {
                        doubleDigitMonth = true;
                        unixTimeStamp = year + "0" + month + day;
                        IsCustomChannel = true;
                    }

                    channel = Path.GetFileNameWithoutExtension(channelName.XmlFileName);
                    var document =
                        XDocument.Load(ConfigurationManager.AppSettings["PastXmlFolderPath"] + GetPastFolder(epgStartDate) + channelName.XmlFileName);

                    filterList = document.Descendants()
                       .DescendantNodes()
                       .OfType<XElement>()
                       .Where(x => x.Name.LocalName.Equals("programme"))
                       .Where(x => x.Attribute("start").Value.Substring(0, doubleDigitMonth ? 8 : 7).Equals(unixTimeStamp)).ToList();

                }
            }

            return GetChannelXml(filterList, channelIdwithoutOffset, channel, offSet, IsCustomChannel, lenght);
        }
        private static string GetDayFolder(int requestedDay)
        {
            var currentDay = DateTime.Today.Day;
            return @"Day"+ Convert.ToString(currentDay - requestedDay) +@"\";
        }

       

        private static string GetPastFolder(DateTime epgStartDate)
        {
            return @"Past7\";
            //string val= @"Day" + (DateTime.Today - epgStartDate).TotalDays;
            //return   @"Day" + (DateTime.Today - epgStartDate).TotalDays + @"\";
        }

        private static XDocument GetChannelXml(List<XElement> programmeList, string channelId, string channelName, int offSet, bool isCustomChannel,string lenght)
        {
               
            var doc = new XDocument();

            doc.Add(new XElement("channel",
                new XAttribute("id", channelId),
                new XAttribute("name", channelName)
                ));

            if (!isCustomChannel)
            {
                foreach (var elem in programmeList)
                {
                    doc.Root.Add(
                        new XElement("programme",
                            new XAttribute("start", ConvertToUnixTime(elem.Attribute("start").Value)),
                            new XAttribute("stop", ConvertToUnixTime(elem.Attribute("stop").Value)),
                            new XAttribute("length", GetLenght(elem.Elements().ElementAt(0).Value,lenght)),
                              

                            new XElement("date", ParserEngine.GetDateFromUnixTimeStamp(elem.Attribute("start").Value)),
                            new XElement("title", elem.Elements().ElementAt(1).Value),
                            new XElement("subTitle", ""),
                            new XElement("desc", elem.Elements().ElementAt(3).Value),
                            new XElement("icon", ""),
                            new XElement("image", "")
                            ));
                }
            }
            else
            {
                foreach (var elem in programmeList)
                {
                    doc.Root.Add(
                        new XElement("programme",
                            new XAttribute("start", ConvertToUnixTime(elem.Attribute("start").Value)),
                            new XAttribute("stop", ConvertToUnixTime(elem.Attribute("stop").Value)),
                            new XAttribute("channel", elem.Attribute("channel").Value),

                            new XElement("date", elem.Elements().ElementAt(0).Value),
                            new XElement("title", elem.Elements().ElementAt(1).Value),
                            new XElement("desc", elem.Elements().ElementAt(2).Value)
                               ));
                }
            }

            if (offSet != 0)
            {
                foreach (var att in doc.Descendants()
                       .DescendantNodes()
                       .OfType<XElement>()
                       .Where(x => x.Name.LocalName.Equals("programme"))
                       .Attributes("start"))
                {
                    att.Value = ParserEngine.GetUnixDateAddingOffset(att.Value, offSet);
                   
                }

                foreach (var att in doc.Descendants()
                    .DescendantNodes()
                    .OfType<XElement>()
                    .Where(x => x.Name.LocalName.Equals("programme"))
                    .Attributes("stop"))
                {
                    att.Value = ParserEngine.GetUnixDateAddingOffset(att.Value, offSet);
                }
            }
             
            return doc;
        }


        private static XDocument GetPastChannelXml(List<XElement> programmeList, string channelId, string channelName, int offSet, bool isCustomChannel,string lenght)
        {

            var doc = new XDocument();

            doc.Add(new XElement("channel",
                new XAttribute("id", channelId),
                new XAttribute("name", channelName)
                ));

            if (!isCustomChannel)
            {
                foreach (var elem in programmeList)
                {
                    doc.Root.Add(
                        new XElement("programme",
                            new XAttribute("start",  elem.Attribute("start").Value),
                            new XAttribute("stop",  elem.Attribute("stop").Value),
                            new XAttribute("length", GetLenght(elem.Attribute("length").Value, lenght)),

                            new XElement("date", elem.Elements().ElementAt(0).Value),
                            new XElement("title", elem.Elements().ElementAt(1).Value),
                            new XElement("subTitle", ""),
                            new XElement("desc", elem.Elements().ElementAt(3).Value),
                            new XElement("icon", ""),
                            new XElement("image", "")
                            ));
                }
            }
            else
            {
                foreach (var elem in programmeList)
                {
                    doc.Root.Add(
                        new XElement("programme",
                            new XAttribute("start",  elem.Attribute("start").Value),
                            new XAttribute("stop",  elem.Attribute("stop").Value),
                            new XAttribute("channel", elem.Attribute("channel").Value),

                            new XElement("date", elem.Elements().ElementAt(0).Value),
                            new XElement("title", elem.Elements().ElementAt(1).Value),
                            new XElement("desc", elem.Elements().ElementAt(2).Value)
                               ));
                }
            }

            if (offSet != 0)
            {
                foreach (var att in doc.Descendants()
                       .DescendantNodes()
                       .OfType<XElement>()
                       .Where(x => x.Name.LocalName.Equals("programme"))
                       .Attributes("start"))
                {
                    att.Value = ParserEngine.GetUnixDateAddingOffset(att.Value, offSet);

                }

                foreach (var att in doc.Descendants()
                    .DescendantNodes()
                    .OfType<XElement>()
                    .Where(x => x.Name.LocalName.Equals("programme"))
                    .Attributes("stop"))
                {
                    att.Value = ParserEngine.GetUnixDateAddingOffset(att.Value, offSet);
                }
            }

            return doc;
        }


        private static string GetLenght(string duration,string length)
        {
            var returnVal = string.Empty;

            if (string.IsNullOrEmpty(length))
                return duration;
            else if (length.Equals("seconds"))
            {
                returnVal = Convert.ToString(Convert.ToInt32(duration)*60);
            }

            return returnVal;
        }


        private static XDocument GetBlankXml(XmlImport channel)
        { 
            var doc = new XDocument(new XElement("channel",
                new XAttribute("id", channel.ChannelId),
                new XAttribute("name", Path.GetFileNameWithoutExtension(channel.XmlFileName))));
             
            return doc;
        }
        public static long ConvertToUnixTime(string datetime)
        {
            //Detect timezone offset Value automatically.
            var timezone = datetime.Substring(datetime.IndexOf("+", StringComparison.Ordinal));
            var hours = 1;
            if (timezone.Equals("+0200"))
                hours = 2;
            else
                hours = 1;


            var timest = GetDateFromUnixTimeStamp(datetime);
            //DateTime dtDateTime = Convert.ToDateTime(timest,null);
            DateTime sTime = new DateTime(1970, 1, 1, hours, 0, 0, DateTimeKind.Utc);

            return (long)(timest - sTime).TotalSeconds;
        }
         
        public static DateTime GetDateFromUnixTimeStamp(string originalTimeSpan)
        {

            var retval = new DateTime();

            try
            {
                if (originalTimeSpan.Contains("+"))
                {
                    originalTimeSpan = originalTimeSpan.Substring(0, originalTimeSpan.IndexOf('+')).Trim();
                }

                //Added on 5th-Jan-2016.
                //Handle single digit months by adding a leading zero. This is a bug in c# according to msdn while converting from UNIX format dates.
                if (originalTimeSpan.Length < 14)
                {
                    var year = originalTimeSpan.Substring(0, 4);
                    originalTimeSpan = year + "0" + originalTimeSpan.Substring(4);
                }

                var originalDate = DateTime.ParseExact(originalTimeSpan, "yyyyMMddHHmmss", null);
                retval = originalDate;
                // retval = originalDate.Year + "-" + originalDate.Month + "-" + originalDate.Day;


            }
            catch (Exception getDateAddingOffsetException)
            { }

            return retval;
        }
         

        /// <summary>
        /// Convert a date time object to Unix time representation.
        /// </summary>
        /// <param name="datetime">The datetime object to convert to Unix time stamp.</param>
        /// <returns>Returns a numerical representation (Unix time) of the DateTime object.</returns>
        

        public static XmlDocument ToXmlDocument(XDocument xDocument)
        {
            var xmlDocument = new XmlDocument();
            using (var xmlReader = xDocument.CreateReader())
            {
                xmlDocument.Load(xmlReader);
            }
            return xmlDocument;
        }

        #region Error Reporting...

        public static bool SendMail(string toEmail, string subject, bool isBodyHtml, string body, bool enableSsl)
        {
            var bSuccess = true;
            var fromEmail = Convert.ToString(ConfigurationManager.AppSettings["SMPT_USER"]);
            var fromPassword = Convert.ToString(ConfigurationManager.AppSettings["SMPT_PASS"]);
            var msg = new MailMessage(fromEmail, toEmail, subject, body) { IsBodyHtml = true };
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

        #endregion Error Reporting...
    }
}
