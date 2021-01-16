using System;
using System.Collections;
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
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.Linq;
using XmlParser.Core;
using XmlParser.Core.DataContext;

namespace XmlParser.Service
{
    /// <summary>
    /// Summary description for FullChannelService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    [ScriptService]
    public class FullChannelService : System.Web.Services.WebService
    {
        public AuthHeader Authentication;
        public static List<string> ExceptionsList;
         
        #region Namespace...

        readonly static XNamespace Xsd = "http://www.w3.org/2001/XMLSchema";
        readonly static XNamespace Xsi = "http://www.w3.org/2001/XMLSchema-instance";
        readonly static XNamespace SchemaLocation = XNamespace.Get("tv:noNamespaceSchemaLocation");

        #endregion Namespace...
        /// <summary>
        /// EPG Xml Service.
        /// </summary>
        /// <param name="channelIds">Comma Separated channel Ids or channel id, with or without offset "channelid[offset value]."</param>
        /// <param name="message">Out message returned by service. "200/OK" if successfull.</param>
        /// <param name="token">Authentication token used to access service in case of a Uri request.</param>
        /// <param name="lenght">lenght in minutes or seconds.</param>
        /// <returns>Single XmlDocument containing the information requested.</returns>
        [ScriptMethod(UseHttpGet = true)]
        [WebMethod]
        [SoapHeader("Authentication", Required = true)]
        public XmlDocument GetChannels(string channelIds, out string message, string format = null, string token = null, string lenght = null,bool IsMultiple=false)
        {
            ExceptionsList = new List<string>();
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
                else if (Authentication.Username.Equals(ConfigurationManager.AppSettings["SVC_USR"]) && Authentication.Password.Equals(ConfigurationManager.AppSettings["SVC_PWD"]))
                {
                    authorization = true;
                }

                if (!isTokenRequest && Authentication != null)
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
                         

                        if(IsMultiple)
                            finalDocument.Add(new XElement("tv", ""));

                        foreach (var item in GetChannels(hashSet, lenght, IsMultiple,format))
                        {
                            if (finalDocument.Root == null)
                                finalDocument.Add(item.Nodes());
                            else
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
                    mailMessage.Append("<br/><strong>Web Service call failed for following" + (ExceptionsList.Count > 1 ? " channels" : " channel") + " : </strong><br/><br/>");
                    var iCount = 1;
                    foreach (var channel in ExceptionsList)
                    {
                        mailMessage.Append(iCount + ".> " + channel + " returned a blank document.<br/><br/>");
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

        public static XmlDocument ToXmlDocument(XDocument xDocument)
        {
            var xmlDocument = new XmlDocument();
            using (var xmlReader = xDocument.CreateReader())
            {
                xmlDocument.Load(xmlReader);
            }
            return xmlDocument;
        }

        private static IEnumerable<XDocument> GetChannels(IEnumerable<string> hashSet, string lenght, bool IsMultiple,string format)
        {
            return hashSet.Select(item => GetChannelTable(item, lenght, IsMultiple, format)).ToList();
        }

        public static XDocument MergeDir(ArrayList xmlArrayList)
        {
            XDocument xdoc = XDocument.Parse("<root></root>");

            foreach (string file in xmlArrayList)
            {
                var xElement = XDocument.Load(file).Root;
                if (xElement != null) if (xdoc.Root != null) xdoc.Root.Add(xElement.Elements());
            }
              
            return xdoc;
        }


        private static XDocument GetChannelTable(string channelIdwithOffset, string length, bool IsMultiple,string format)
        {
            var channel = string.Empty;
            string channelIdwithoutOffset;
            var offSet = 0;
            var isCustomChannel = false;
            var finalDoc = new XDocument();

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

            using (var context = new EPGDataModel())
            {
                var channelName = context.XmlImports.FirstOrDefault(x => x.ChannelId.Contains(channelIdwithoutOffset));

                if (channelName != null)
                {
                    isCustomChannel = channelName.SourceUrl.Equals("Custom Channel");

                    channel = Path.GetFileNameWithoutExtension(channelName.XmlFileName);

                    var arrXmlsArrayList = new ArrayList
                    {
                        ConfigurationManager.AppSettings["XML_PATH"] + channelName.XmlFileName,
                        ConfigurationManager.AppSettings["PastXmlFolderPath"] + @"Past7\" + channelName.XmlFileName
                    };

                    var doc = XDocument.Load(arrXmlsArrayList[0].ToString());


                    finalDoc = XDocument.Load(arrXmlsArrayList[0].ToString());

                    #region Added for Past Xml...

                    if (File.Exists(arrXmlsArrayList[1].ToString()))
                    {
                        var docPastXml = XDocument.Load(arrXmlsArrayList[1].ToString());

                        docPastXml.Root.Add(doc.Descendants()
                           .DescendantNodes()
                           .OfType<XElement>()
                           .Where(n => n.Name.LocalName.Equals("programme")));


                        var filterList = docPastXml.Descendants()
                           .DescendantNodes()
                           .OfType<XElement>()
                           .Where(x => x.Name.LocalName.Equals("programme"));

                        var edoc = new XDocument();
                        edoc.Add(new XElement("feed", ""));
                        edoc.Root.Add(filterList);

                        var xElement = XElement.Parse(edoc.ToString());

                        var finalList = xElement.Elements().Distinct(new MyComparer()).ToList();

                        var docresult = new XDocument();
                        docresult.Add(new XElement("tv",
                            new XAttribute(XNamespace.Xmlns + "xsd", Xsd),
                            new XAttribute(XNamespace.Xmlns + "xsi", Xsi)
                             , new XAttribute(Xsi + "schemaLocation", SchemaLocation)));

                        docresult.Root.Add(new XElement("channel",
                           new XAttribute("id", channelIdwithoutOffset),
                           new XAttribute("name", Path.GetFileNameWithoutExtension(arrXmlsArrayList[1].ToString())),
                                new XElement("display-name", Path.GetFileNameWithoutExtension(arrXmlsArrayList[1].ToString())))
                           );

                        foreach (var item in finalList)
                        {
                            docresult.Root.Add(new XElement("programme",
                                new XAttribute("channel", item.Attribute("channel").Value),
                                new XAttribute("start", item.Attribute("start").Value),
                                new XAttribute("stop", item.Attribute("stop").Value),
                              GetNodes(item)
                                ));
                        }


                        var xml = ToXmlDocument(docresult);
                        var result = new XmlStripper().RemoveAllNamespaces(xml.DocumentElement);

                        finalDoc = ToXDocument(result);
                    }

                    #endregion Added for Past Xml...
                     
                }
            }

            var returnList = finalDoc.Descendants()
                .DescendantNodes()
                .OfType<XElement>()
                .Where(x => x.Name.LocalName.Equals("programme")).ToList();
             
            returnList = returnList.OrderBy(s => (string)s.Attribute("start")).ToList();
             
            return GetChannelXml(returnList, channelIdwithoutOffset, channel, offSet, isCustomChannel, length, IsMultiple,format);
             
        }

        private static XDocument GetChannelXml(List<XElement> programmeList, string channelId, string channelName, int offSet, bool isCustomChannel, string lenght,bool IsMultiple,string format)
        {

            var doc = new XDocument();


            if (IsMultiple)
            {
                doc.Add(new XElement("channel",
                   new XAttribute("id", channelId),
                   new XAttribute("name", channelName)
                   ));
            }
            else
               doc.Add(new XElement("tv", ""));
           

            try
            {
                if (!isCustomChannel)
                {
                    foreach (var elem in programmeList)
                    {

                        if (format!=null)
                        {
                            doc.Root.Add(
                                new XElement("programme",
                                    new XAttribute("start", ConvertToUnixTime(elem.Attribute("start").Value)),
                                    new XAttribute("stop", ConvertToUnixTime(elem.Attribute("stop").Value)),
                                    new XAttribute("length", GetLenght(elem.Elements().ElementAt(0).Value, lenght)),


                                    new XElement("date", ParserEngine.GetDateFromUnixTimeStamp(elem.Attribute("start").Value)),
                                    new XElement("title", elem.Elements().ElementAt(1).Value),
                                    new XElement("subTitle", ""),
                                    new XElement("desc", elem.Elements().ElementAt(3).Value),
                                    new XElement("icon", ""),
                                    new XElement("image", "")
                                    ));
                        }
                        else { 
                        doc.Root.Add(
                            new XElement("programme",
                                new XAttribute("start", elem.Attribute("start").Value),
                                new XAttribute("stop", elem.Attribute("stop").Value),
                                new XAttribute("channel", elem.Attribute("channel").Value),
                                new XAttribute("length", GetLenght(elem.Elements().ElementAt(0).Value, lenght)),


                                new XElement("date", ParserEngine.GetDateFromUnixTimeStamp(elem.Attribute("start").Value)),
                                new XElement("title", elem.Elements().ElementAt(1).Value),
                                new XElement("subTitle", ""),
                                new XElement("desc", elem.Elements().ElementAt(3).Value),
                                new XElement("icon", ""),
                                new XElement("image", "")
                                ));
                        }
                    }
                }
                else
                {
                    foreach (var elem in programmeList)
                    {
                        doc.Root.Add(
                            new XElement("programme",
                                new XAttribute("start", elem.Attribute("start").Value),
                                new XAttribute("stop", elem.Attribute("stop").Value),
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

                        if (format != null) { 
                            att.Value = ParserEngine.GetUnixDateAddingOffset(att.Value, offSet);
                            att.Parent.Element("date").Value = UnixTimeStampToDateTime(Convert.ToInt64(att.Value)).ToString("yyyy-MM-dd");
                        }
                        else {

                            att.Value = ParserEngine.GetDateAddingOffset(att.Value, offSet);
                            string originalTimeSpan = ParserEngine.GetDateAddingOffset(att.Value, 0);
                             
                            if (originalTimeSpan.Contains("+"))
                            {
                                originalTimeSpan = originalTimeSpan.Substring(0, originalTimeSpan.IndexOf('+')).Trim();
                            }

                            var originalDate = DateTime.ParseExact(originalTimeSpan, "yyyyMMddHHmmss", CultureInfo.InvariantCulture).ToUniversalTime();
                             
                            att.Parent.Element("date").Value = originalDate.ToString("yyyy-MM-dd");
                        }
                          
                    }
                     
                    foreach (var att in doc.Descendants()
                        .DescendantNodes()
                        .OfType<XElement>()
                        .Where(x => x.Name.LocalName.Equals("programme"))
                        .Attributes("stop"))
                    {
                        if (format != null)
                        {
                            att.Value = ParserEngine.GetUnixDateAddingOffset(att.Value, offSet);
                        }
                        else
                        {
                            att.Value = ParserEngine.GetDateAddingOffset(att.Value, offSet);
                            
                        }
                    }
                }
            }
            catch(Exception ex)
            {

            }
            

            return doc;
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToUniversalTime();
            return dtDateTime;
        }

        private static string GetLenght(string duration, string length)
        {
            var returnVal = string.Empty;

            if (string.IsNullOrEmpty(length))
                return duration;
            else if (length.Equals("seconds"))
            {
                returnVal = Convert.ToString(Convert.ToInt32(duration) * 60);
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
            //Detect timezone offset Value.
            var timezone = datetime.Substring(datetime.IndexOf("+", StringComparison.Ordinal));
            var hours = 1;
            if (timezone.Equals("+0200"))
                hours = 2;
            else
                hours = 1;


            var timest = GetDateFromUnixTimeStamp(datetime);
            
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

                 
                if (originalTimeSpan.Length < 14)
                {
                    var year = originalTimeSpan.Substring(0, 4);
                    originalTimeSpan = year + "0" + originalTimeSpan.Substring(4);
                }

                var originalDate = DateTime.ParseExact(originalTimeSpan, "yyyyMMddHHmmss", null);
                retval = originalDate;
                 
            }
            catch (Exception getDateAddingOffsetException)
            { }

            return retval;
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

        private static object GetNodes(XElement item)
        {
            XNode[] objListObjects = new XNode[item.Nodes().Count()];

            int i = 0;
            foreach (var node in item.Nodes())
            {
                objListObjects[i] = node;
                i++;
            }

            return objListObjects;
        }

       
        public static XDocument ToXDocument(XmlDocument xmlDocument)
        {
            using (var nodeReader = new XmlNodeReader(xmlDocument))
            {
                nodeReader.MoveToContent();
                return XDocument.Load(nodeReader);
            }
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

    public class XmlStripper
    {
        public XmlDocument RemoveAllNamespaces(XmlNode documentElement)
        {
            var xmlnsPattern = "\\s+xmlns\\s*(:\\w)?\\s*=\\s*\\\"(?<url>[^\\\"]*)\\\"";
            var outerXml = documentElement.OuterXml;
            var matchCol = Regex.Matches(outerXml, xmlnsPattern);
            foreach (var match in matchCol)
                outerXml = outerXml.Replace(match.ToString(), "");

            var result = new XmlDocument();
            result.LoadXml(outerXml);

            return result;
        }

        public XmlNode Strip(XmlNode documentElement)
        {
            var namespaceManager = new XmlNamespaceManager(documentElement.OwnerDocument.NameTable);
            foreach (var nspace in namespaceManager.GetNamespacesInScope(XmlNamespaceScope.All))
            {
                namespaceManager.RemoveNamespace(nspace.Key, nspace.Value);
            }

            return documentElement;
        }
    }
    public class MyComparer : IEqualityComparer<XElement>
    {
        public bool Equals(XElement x, XElement y)
        {
            return x.Attribute("start").Value == y.Attribute("start").Value;
        }

        public int GetHashCode(XElement obj)
        {
            return obj.Attribute("start").Value.GetHashCode();
        }
    }
}
