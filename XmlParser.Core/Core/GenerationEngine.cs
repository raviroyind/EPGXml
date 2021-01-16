using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using XmlParser.Core.DataContext;

namespace XmlParser.Core.Core
{
    public static class GenerationEngine
    {
        static readonly List<string> TimetableXmlList = new List<string>();
        public static List<string> GenerateTimeTableXml(ActiveChannel channel)
        { 
            TimetableXmlList.Clear();

            #region Namespace...

            XNamespace xsd = "http://www.w3.org/2001/XMLSchema";
            XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
            
            #endregion Namespace...

            #region EPG Dates...

            var minDate = DateTime.Today.AddDays(-7);
            
            var maxDate = DateTime.Today.AddDays(7);
             
            #endregion EPG Dates...

            var doc = new XDocument();

            doc.Add(new XElement("feed",
                new XAttribute(XNamespace.Xmlns + "xsi", xsi),
                new XAttribute(XNamespace.Xmlns + "xsd", xsd))
                );

            if (doc.Root != null)
            {
                doc.Root.Add(new XElement("channels",
                    new XElement("channel",
                        new XAttribute("display-name", channel.ChannelName + ".xml"),
                        new XAttribute("display-number", "1"),
                        new XAttribute("id", "")
                        )));

                doc.Root.Add(GetProgrammeNodes(channel.ChannelName, minDate, maxDate));
            }

            doc.Save(ConfigurationManager.AppSettings["OutputFolderPath"]+ channel.ChannelName +".xml");

            TimetableXmlList.Add(ConfigurationManager.AppSettings["OutputFolderPath"] + channel.ChannelName + ".xml," + minDate+","+maxDate);
            return TimetableXmlList;
        }

        private static object[] GetProgrammeNodes(string channel, DateTime minDate, DateTime maxDate)
        {
            object[] listProgrammeNodes = new object[360];

            try
            {
                var stop = 1;
                for (var i = 0; i <= 360; i++)
                {
                    listProgrammeNodes[i] =
                        new XElement("programme",
                            new XAttribute("channel", channel),
                            new XAttribute("start", GetDateAddingHours(minDate, i)),
                            new XAttribute("stop", GetDateAddingHours(minDate, stop)),
                        new XElement("date", GetDateOnly(minDate, i)),
                        new XElement("title", GetTimeSpan(minDate, i) + "-" + GetTimeSpan(minDate, stop)),
                        new XElement("description", GetTimeSpan(minDate, i) + " - " + GetTimeSpan(minDate, stop) + " (60min)")
                            );

                    stop++;
                }
            }
            catch (Exception ex)
            {  }
             
            return listProgrammeNodes;
        }

        private static string GetDateAddingHours(DateTime minDate, int hourstoAdd)
        {
            minDate = minDate.AddHours(hourstoAdd);
            var originalDate = minDate.ToString("yyyyMMddHHmmss") + " +0200";
            return originalDate;
        }

        private static int GetTimeSpan(DateTime minDate, int hourstoAdd)
        {
           var  date = minDate.AddHours(hourstoAdd);
           return date.Hour;
        }

        private static string GetDateOnly(DateTime minDate, int hourstoAdd)
        {
            minDate = minDate.AddHours(hourstoAdd);
            var originalDate = minDate.ToString("yyyyMMddHHmmss") + " +0200";
            var date = DateTimeOffset.ParseExact(originalDate, "yyyyMMddHHmmss zzz", CultureInfo.InvariantCulture);
            return date.Date.ToString("yyyy-MM-dd");
        }
    }
}
