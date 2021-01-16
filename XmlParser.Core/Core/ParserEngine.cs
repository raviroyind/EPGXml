using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Ionic.Zip;
using log4net;
using XmlParser.Core.DataContext;
using XmlParser.Core.XmlSchema;

namespace XmlParser.Core
{

    public static  class ParserEngine
    {
        #region Variables...

        private static readonly ILog Logger = LogManager.GetLogger(typeof(ParserEngine));

        static readonly List<string> SourceXmlList = new List<string>();
        static readonly List<string> OutputXmlList = new List<string>();

        static DateTime _startDate = new DateTime();
        static DateTime _stopDate = new DateTime();

        private static readonly string ZipFolderPath = ConfigurationManager.AppSettings["ZipFolderPath"];
        private static readonly string SourceXmlFolderPath = ConfigurationManager.AppSettings["SourceXmlFolderPath"];
        private static readonly string OutputFolderPath = ConfigurationManager.AppSettings["OutputFolderPath"];

        #endregion Variables...
         
       #region Functions...

        /// <summary>
        /// Generates main output xml.
        /// </summary>
        /// <param name="sourceXml"></param>
        /// <param name="srno"></param>
        /// <param name="epgEnd"></param>
        /// <param name="newChannelName"></param>
        /// <param name="newOffset"></param>
        /// <param name="epgStart"></param>
        public static List<string> GenerateOutputXml(string sourceXml, long srno,out DateTime epgStart,out DateTime epgEnd, string newChannelName = null, int newOffset = 0)
       {
            OutputXmlList.Clear();
            epgStart=new DateTime();
            epgEnd = new DateTime();

           var document = XDocument.Load(sourceXml);

           var _tv = new tv();
           var channelNode1 = document.Descendants("channel");

           var xElements = channelNode1 as XElement[] ?? channelNode1.ToArray();
           for (var i = 0; i <= xElements.Length - 1; i++)
           {
               var elementChannel = xElements[i];
               var elementchannelId = elementChannel.Attributes("id").First().Value;


               var progArray = document.Descendants("programme")
                   .Where(x => (string)x.Attribute("channel").Value == elementchannelId);

               #region Channel...
               var id = string.Empty;
               var channelName = string.Empty;
               var queryChannelName = string.Empty;
               const string formatString = "yyyyMMddHHmmss";
               var outputFileName = string.Empty;

               #region Channel Name...

               if (!string.IsNullOrEmpty(newChannelName))
               {
                   channelName = newChannelName;
                  
                   id = xElements[i].Attributes("id").First().Value;
                   queryChannelName = xElements[i].Value;
                   if (string.IsNullOrEmpty(queryChannelName))
                   {
                       queryChannelName = xElements[i].Attributes("display-name").First().Value;
                   }

                   try
                   {
                       id = xElements[i].Attributes("id").First().Value;
                       outputFileName = xElements[i].Value;

                       if (string.IsNullOrEmpty(outputFileName))
                       {
                           outputFileName = xElements[i].Attributes("display-name").First().Value;
                       }
                   }
                   catch (Exception)
                   {
                       var channelNode = document.Descendants("channel").First();
                       outputFileName = channelNode.Value;
                       if (string.IsNullOrEmpty(channelName))
                       {
                           outputFileName = xElements[i].Attributes("display-name").First().Value;
                       }
                   }
               }
               else
               {
                   try
                   {
                       id = xElements[i].Attributes("id").First().Value;

                       channelName = xElements[i].Value;
                       queryChannelName = channelName;
                       //Nov-19
                       if (channelName.Contains("http://"))
                       {
                           channelName = channelName.Substring(0, channelName.IndexOf("http:", StringComparison.Ordinal));
                       }

                       if (string.IsNullOrEmpty(channelName))
                       {
                           channelName = xElements[i].Attributes("display-name").First().Value;
                       }
                   }
                   catch (Exception)
                   {
                       var channelNode = document.Descendants("channel").First();
                       channelName = channelNode.Value;
                       if (string.IsNullOrEmpty(channelName))
                       {
                           channelName = xElements[i].Attributes("display-name").First().Value;
                       }
                   }

                   outputFileName = channelName;

               }

               #endregion Channel Name...

               //1.> Nov-19 - Remove any Illegeal characters from filename.


               //2.> Nov-25 - Task ID:  1647 (duplicate channel bug ).
               if (outputFileName.Contains("http://"))
               {
                   outputFileName = outputFileName.Substring(0, outputFileName.IndexOf("http://", StringComparison.Ordinal));
               }

               outputFileName = GetSafeFilename(outputFileName);
               queryChannelName = outputFileName;
               //Check if current channe is active.
               using (var dataContext = new EPGDataModel())
               {
                   var channelList =
                       dataContext.SourceURLs.Find(srno)
                           .ActiveChannels.FirstOrDefault(c => c.ChannelName.Equals(queryChannelName) && c.IsActive && c.IsManuallyAdded.Equals(false));//26th-Jan IsManuallyAdded Condition added.

                   if (channelList == null)
                       continue;
               }
               //End

                
               var tvChannel = new tvChannel
               {
                   id = id,
                   displayname = channelName,

               };

               _tv.channel = tvChannel;

               #endregion Channel...


               #region Programme Nodes...

               var sourceXElements = progArray.ToList();

               //If no programe nodes move to next channel.
               if (sourceXElements.Count == 0)
                   continue;


               var _tvProgrammes = new tvProgramme[sourceXElements.Count];

               var iNodeCount = 0;

               foreach (var item in sourceXElements)
               {
                   #region Minutes...

                   var startTime = item.Attributes("start").First().Value;
                   var epgStartOffset = startTime;


                   if (startTime.Contains("+"))
                       startTime = startTime.Substring(0, startTime.IndexOf('+'));

                   var stopTime = item.Attributes("stop").First().Value;

                   if (stopTime.Contains("+"))
                       stopTime = stopTime.Substring(0, stopTime.IndexOf('+'));

                   var duration = DateTime.ParseExact(stopTime.Trim(), formatString, CultureInfo.InvariantCulture).Subtract(DateTime.ParseExact(startTime.Trim(), formatString, CultureInfo.InvariantCulture)).TotalMinutes;

                   #endregion Minutes...

                   #region Credits...

                   tvProgrammeName[] tvProgrammeNames = null;

                   if (item.Element("credits") != null)
                   {

                       XElement creditElement = item.Element("credits");

                       if (creditElement != null)
                       {
                           var creditsArray = creditElement.Nodes().ToList();

                           tvProgrammeNames = new tvProgrammeName[creditsArray.Count];


                           for (var x = 0; x <= creditsArray.Count - 1; x++)
                           {
                               XNode node = creditsArray[x];
                               XElement element = (XElement)node;

                               tvProgrammeNames[x] = new tvProgrammeName
                               {
                                   function = element.Name.LocalName,
                                   Value = element.Value
                               };
                           }

                       }
                   }

                   #endregion Credits...


                   #region Image...

                   var imageSrc = "";

                   if (item.Element("icon") != null)
                   {
                       var imgElement = item.Element("icon");

                       if (imgElement != null && imgElement.Attribute("src") != null)
                       {
                           imageSrc = imgElement.Attribute("src").Value;
                       }
                   }


                   #endregion Image...

                   _tvProgrammes[iNodeCount] = new tvProgramme
                   {
                       channel = channelName,
                       start = GetDateAddingOffset(item.Attributes("start").First().Value, newOffset),
                       stop = GetDateAddingOffset(item.Attributes("stop").First().Value, newOffset),

                       length = new tvProgrammeLength
                       {
                           Value = ushort.Parse(Convert.ToString(duration, CultureInfo.InvariantCulture)),
                           units = "minutes"
                       },
                       title = item.Element("title") != null ? RemoveUnwantedText(item.Element("title").Value) : "",
                       category = item.Element("category") != null ?RemoveUnwantedText( item.Element("category").Value) : "",
                       desc = item.Element("desc") != null ? RemoveUnwantedText(item.Element("desc").Value) : "",
                       credits = tvProgrammeNames,
                       image = imageSrc
                   };

                   iNodeCount++;
               }


               _tv.programme = _tvProgrammes;


               var startDt = _tvProgrammes.First().start;

               if (startDt.Contains("+"))
                   startDt = startDt.Substring(0, startDt.IndexOf('+')).Trim();

               var stopDt = _tvProgrammes.Reverse().First().stop;

               if (stopDt.Contains("+"))
                   stopDt = stopDt.Substring(0, stopDt.IndexOf('+')).Trim();

               try
               {
                   //Handle single digit months by adding a leading zero. This is a bug in c# according to msdn while converting from UNIX format dates.
                   var tempStart = startDt;
                   var tempEnd = stopDt;
                   
                   if (tempStart.Length < 14)
                   {
                       var year = tempStart.Substring(0, 4);
                       tempStart = year + "0" + tempStart.Substring(4);
                   }

                   if (tempEnd.Length < 14)
                   {
                       var year = tempEnd.Substring(0, 4);
                       tempEnd = year + "0" + tempEnd.Substring(4);
                   }

                   _startDate = DateTime.ParseExact(tempStart, formatString, CultureInfo.InvariantCulture);
                   _stopDate = DateTime.ParseExact(tempEnd, formatString, CultureInfo.InvariantCulture);

                   epgStart = _startDate;
                   epgEnd = _stopDate;
               }
               catch (Exception)
               { }

               #endregion Programme Nodes...


               _tv.Save( OutputFolderPath + outputFileName + ".xml");


               #region Generate Additional Xml...

               var _tv2 = new XmlParser.Core.XmlSchema.SecondOutput.tv();
               XmlParser.Core.XmlSchema.SecondOutput.tvProgramme[] _tvProgrammes2 = new XmlParser.Core.XmlSchema.SecondOutput.tvProgramme[sourceXElements.Count];


               iNodeCount = 0;
               foreach (var item in sourceXElements)
               {
                   #region Minutes...

                   var startTime = item.Attributes("start").First().Value;
                   var epgStartOffset = startTime;


                   if (startTime.Contains("+"))
                       startTime = startTime.Substring(0, startTime.IndexOf('+'));

                   var stopTime = item.Attributes("stop").First().Value;

                   if (stopTime.Contains("+"))
                       stopTime = stopTime.Substring(0, stopTime.IndexOf('+'));

                   var duration = DateTime.ParseExact(stopTime.Trim(), formatString, CultureInfo.InvariantCulture).Subtract(DateTime.ParseExact(startTime.Trim(), formatString, CultureInfo.InvariantCulture)).TotalMinutes;


                   // var date = DateTime.ParseExact(startTime, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);

                   #endregion Minutes...

                   _tvProgrammes2[iNodeCount] = new XmlParser.Core.XmlSchema.SecondOutput.tvProgramme()
                   {
                       channel = channelName,
                       start = GetDateAddingOffset(item.Attributes("start").First().Value, newOffset),
                       stop = GetDateAddingOffset(item.Attributes("stop").First().Value, newOffset),
                       title = item.Element("title") != null ? RemoveUnwantedText(item.Element("title").Value) : "",
                       desc = item.Element("desc") != null ? RemoveUnwantedText(item.Element("desc").Value) : "",
                       subTitle = item.Element("sub-title") != null ? RemoveUnwantedText(item.Element("sub-title").Value) : "",
                       date = DateTime.Today,
                       originid = "",//Find
                       reserve = "no"
                   };

                   iNodeCount++;
               }


               _tv2.programme = _tvProgrammes2;




               _tv2.Save(OutputFolderPath + outputFileName + "_additional" + ".xml");

               #endregion Generate Additional Xml...


               OutputXmlList.Add("../Output/" + outputFileName + ".xml" + ","
                   + _startDate.Date.ToString("yyyy-MM-dd",CultureInfo.InvariantCulture) + ","
                   + _stopDate.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) + ","
                   + "../Output/" + outputFileName + "_additional" + ".xml" + ","
                   + "../SourceXmlFiles/" + Path.GetFileName(sourceXml)
                   );
           }

            return OutputXmlList;
       }

        private static string RemoveUnwantedText(string inValue)
        { 
            if (inValue.Contains("(?)"))
                inValue = inValue.Replace("(?)","");

            if (inValue.Contains("(r)"))
                inValue = inValue.Replace("(r)","");

           if (inValue.Contains("(c)"))
                inValue = inValue.Replace("(c)","");

            if (inValue.Contains("(n)"))
                inValue = inValue.Replace("(n)","");

            if (inValue.Contains("(Lire la suite)"))
                inValue = inValue.Replace("(Lire la suite)", "");

            return inValue;
        }

        
       /// <summary>
       /// Add Time Offset to Start & End attributes of output xml based on query string value for offset.
       /// </summary>
       /// <param name="originalTimeSpan">Original Start/ Stop value.</param>
       /// <param name="newOffset">Hours Offset to be added to original timespan.</param>
       /// <returns>String representation of original DateTime value after adding hours offset.</returns>
       [Obsolete("Use function Below")]
       private static string GetDateAddingOffsetOld(string originalTimeSpan, int newOffset)
       {
           var dtAdditionalValue = string.Empty;

           if (originalTimeSpan.Contains("+"))
           {
               dtAdditionalValue = originalTimeSpan.Substring(originalTimeSpan.IndexOf('+'));
               originalTimeSpan = originalTimeSpan.Substring(0, originalTimeSpan.IndexOf('+')).Trim();
           }

           var originalDate = DateTime.ParseExact(originalTimeSpan, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);

           var newDate = originalDate.AddHours(newOffset);

           var retval = newDate.Year.ToString() + newDate.Month.ToString() + newDate.Day.ToString("00").ToString() +
                          newDate.Hour.ToString("00") + newDate.Minute.ToString("00") + newDate.Second.ToString("00");

           return retval + " " + dtAdditionalValue;
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

           var originalDate = DateTime.ParseExact(originalTimeSpan, "yyyyMMddHHmmss", CultureInfo.InvariantCulture).ToUniversalTime();

           var newDate = originalDate.AddHours(newOffset).ToUniversalTime();

            var retval = newDate.Year.ToString() + 
                        (newDate.Month.ToString().Length.Equals(1) ? "0" + newDate.Month.ToString() : newDate.Month.ToString()) + 
                        newDate.Day.ToString("00").ToString() +
                        newDate.Hour.ToString("00") + 
                        newDate.Minute.ToString("00") + 
                        newDate.Second.ToString("00");
             
            return retval + " " + dtAdditionalValue;
       }

       /// <summary>
       /// Add Time Offset to Start & End attributes of output xml based on query string value for offset.
       /// </summary>
       /// <param name="originalTimeSpan">Original Start/ Stop value.</param>
       /// <param name="newOffset">Hours Offset to be added to original timespan.</param>
       /// <returns>String representation of original DateTime value after adding hours offset.</returns>
       public static string GetUnixDateAddingOffset(string originalTimeSpan, int newOffset)
       {

           var dtAdditionalValue = string.Empty;

             
           var originalDate = UnixTimeToDateTime(Convert.ToInt64(originalTimeSpan));

           var newDate = originalDate.AddHours(newOffset);

           var retval = newDate.Year.ToString() + newDate.Month.ToString() + newDate.Day.ToString("00").ToString() +
                          newDate.Hour.ToString("00") + newDate.Minute.ToString("00") + newDate.Second.ToString("00");

           return ConvertToUnixTime(retval).ToString();
       }


       public static string GetDateFromUnixTimeStamp(string originalTimeSpan)
       {

           string retval = string.Empty;

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

               var originalDate = DateTime.ParseExact(originalTimeSpan, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);

               retval = originalDate.Year + "-" + originalDate.Month + "-" + originalDate.Day;

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
        public static long ConvertToUnixTime(string datetime)
        {
            var timest = GetDateFromUnixTimeStamp(datetime);
            DateTime dtDateTime = Convert.ToDateTime(timest);
            DateTime sTime = new DateTime(1970, 1, 1, 2, 0, 0, DateTimeKind.Utc);

            return (long) (dtDateTime - sTime).TotalSeconds;
              
        }

        /// <summary>
       /// Convert Unix time value to a DateTime object.
       /// </summary>
       /// <param name="unixtime">The Unix time stamp you want to convert to DateTime.</param>
       /// <returns>Returns a DateTime object that represents value of the Unix time.</returns>
       public static DateTime UnixTimeToDateTime(long unixtime)
       {
           DateTime sTime = new DateTime(1970, 1, 1, 2, 0, 0, DateTimeKind.Utc);
           return sTime.AddSeconds(unixtime);
       }

       private static string DownloadFile(string sourceUrl, string extension)
       {
           sourceUrl = sourceUrl.Trim();
           var file = GetSafeFilename(Path.GetFileNameWithoutExtension(sourceUrl));
           var saveLocation = ZipFolderPath + file + extension;

           saveLocation = saveLocation;

           using (var webClt = new WebClient())
           {
               try
               {
                   webClt.DownloadFile(sourceUrl, saveLocation);
               }
               catch (Exception downloadFileException)
               {
                   Logger.Error("Error occured - DownloadFile | Error message-" + downloadFileException.Message);
                   if (downloadFileException.InnerException != null)
                       Logger.Error("Inner Exception-" + downloadFileException.InnerException.Message);

               }
           }

           return ZipFolderPath + file + extension;
       }

       private static string GetSafeFilename(string illegal)
       {
           var regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
           var r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
           illegal = r.Replace(illegal, "");

           return illegal;
       }

       /// <summary>
       /// 
       /// </summary>
       /// <param name="zipFileName"></param>
       /// <param name="outputDirectory"></param>
       private static void ExtractFileToDirectory(string zipFileName, string outputDirectory)
       {
           ClearDirectory(outputDirectory);

           try
           {
               var zip = ZipFile.Read(zipFileName);

               Logger.Info("// Entered ExtractFileToDirectory.  Zip File Name IS -" + zipFileName + " & output directory is - " + outputDirectory);

               zip.ExtractAll(outputDirectory, ExtractExistingFileAction.OverwriteSilently);


               var files = Directory.GetFiles(outputDirectory, "*.xml", SearchOption.TopDirectoryOnly);

               foreach (var xmlFile in files)
               {
                   SourceXmlList.Add(xmlFile);
               }
           }
           catch (Exception zipExTractionException)
           {
               Logger.Error("Error occured while running ExtractFileToDirectory function. Error Message - " +
                            zipExTractionException.Message);

               if (zipExTractionException.InnerException != null)
                   Logger.Error("Error InnerException - " +
                            zipExTractionException.InnerException.Message);
           }
       }


       /// <summary>
       /// Clean destination directory for new set of source xmls.
       /// </summary>
       /// <param name="path">Path where xmls will be extracted.</param>
       private static void ClearDirectory(string path)
       {
           Logger.Info("// Running function ClearDirectory.");
           foreach (var file in Directory.GetFiles(path).Where(file => !string.IsNullOrEmpty(file)))
           {
               File.Delete(file);
           }

           Logger.Info("// Function ClearDirectory Completed.");
       }
 

       #endregion Functions...

    }
}
