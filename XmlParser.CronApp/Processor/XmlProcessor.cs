using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Linq;
using System.Xml.Schema;
using Ionic.Zip;
using log4net;
using log4net.Config;
using XmlParser.Core;
using XmlParser.Core.Core;
using XmlParser.Core.DataContext;
using XmlParser.Core.XmlSchema;

namespace XmlGeneration.CronApp
{
    public static class XmlProcessor
    {
        #region Variables...

        private static readonly ILog logger = LogManager.GetLogger(typeof(XmlProcessor));

        static readonly List<string> SourceXmlList = new List<string>();
        static readonly List<string> OutputXmlList = new List<string>();
        static readonly List<string> TimetableXmlList = new List<string>();

        static DateTime _startDate = new DateTime();
        static DateTime _stopDate = new DateTime();

        private static readonly string ZipFolderPath = ConfigurationManager.AppSettings["ZipFolderPath"];
        private static readonly string SourceXmlFolderPath = ConfigurationManager.AppSettings["SourceXmlFolderPath"];
        private static readonly string OutputFolderPath = ConfigurationManager.AppSettings["OutputFolderPath"];

        #endregion Variables...



        public static void ProcessXml()
        {
            XmlConfigurator.Configure();

            SourceXmlList.Clear();
            OutputXmlList.Clear();
             
            using (var dataContext = new EPGDataModel())
            {
                List<SourceURL> listToProcess=new List<SourceURL>();
                try
                {
                    logger.Info("1.> Getting List of Source from Database");
                    listToProcess = dataContext.SourceURLs.Where(s => s.IsActive).ToList();
                    logger.Info("2.> Retrived List of Source from Database. Total Sources - " + listToProcess.Count);
                }
                catch (Exception exDataException)
                {
                    logger.Error("Error occured while retriving List of Source from Database. Error Message - " + exDataException.Message);

                    if(exDataException.InnerException !=null)
                        logger.Error("Inner Exception - " + exDataException.InnerException.Message);
                }
                
                foreach (var source in listToProcess)
                {

                    #region Xml Generation...

                    try
                    {
                        logger.Info("ravi");
                        logger.Info("// Proccessing Source - " + source.Url);
                        var extension = source.Type.ToLower().Equals("zip") ? ".zip" : ".xml";
                        logger.Info("// File extension is - " + extension);

                        var fileName = DownloadFile(source.Url, extension);
                        logger.Info("// File Name extension is - " + fileName);
                        
                        if (!string.IsNullOrEmpty(fileName))
                        {
                            if (Path.GetExtension(fileName).Contains(".zip")) //Zip Archive.
                            {
                                logger.Info("// Source is a zip archive.");
                                //extract xmls from zip archive.
                                ExtractFileToDirectory(fileName, SourceXmlFolderPath);
                               
                                logger.Info("// Çompleted extraction of zip archive. Total files extracted - "+ SourceXmlList.Count);
                                 
                            }
                            else if (Path.GetExtension(fileName).Contains(".xml")) //Single Xml.
                            {
                                logger.Info("// Source is a xml file.");
                                //ClearDirectory(Environment.CurrentDirectory + @"\SourceXmlFiles\");

                                try
                                {
                                    logger.Info("// Checking if file " + Path.GetFileName(fileName) + " alredy exists in Source Xml Folder.");
                                    if (File.Exists(SourceXmlFolderPath + Path.GetFileName(fileName)))
                                        File.Delete(SourceXmlFolderPath + Path.GetFileName(fileName));

                                    logger.Info("// Copying File " + Path.GetFileName(fileName) + " to Source Xml Folder.");
                                    File.Copy(fileName, SourceXmlFolderPath + Path.GetFileName(fileName));
                                    logger.Info("// Completed Copying File " + Path.GetFileName(fileName) + " to Source Xml Folder.");
                                    SourceXmlList.Add(SourceXmlFolderPath + Path.GetFileName(fileName));
                                    logger.Info("// Çompleted Adding file to SourceXmlList - Total Count is - " + SourceXmlList.Count);
                                }
                                catch (Exception singleXmlException)
                                {
                                     logger.Error("Error occured while copying single xml. Error - " + singleXmlException.Message);

                                    if(singleXmlException.InnerException !=null)
                                        logger.Error("Inner Exception Error - " + singleXmlException.InnerException.Message);
                                }
                            }


                            foreach (var xmlFile in SourceXmlList)
                            {

                                var sourceUri = new Uri(source.Url);
                                var newChannelName = string.Empty;
                                var newOffset = 0;

                                try
                                {
                                    newChannelName = HttpUtility.ParseQueryString(sourceUri.Query).Get("channelname");
                                    int.TryParse(HttpUtility.ParseQueryString(sourceUri.Query).Get("offset"), out newOffset);
                                }
                                catch (Exception)
                                {
                                    // ignored
                                }

                                logger.Info("Calling  GenerateOutputXml(" + xmlFile + ", " + newChannelName + ", " + newOffset + ")");

                                OutputXmlList.AddRange(ParserEngine.GenerateOutputXml(xmlFile, source.Srno, out _startDate, out _stopDate, newChannelName, newOffset));
                                  
                                logger.Info("Completed  GenerateOutputXml(" + xmlFile + ", " + newChannelName + ", " + newOffset + ")");
                                 
                            }
                        }

                        SourceXmlList.Clear();
                    }
                    catch (Exception)
                    {

                    }

                    #endregion Xml Generation...

                    #region Hourly Timetable Xml...

                    //try
                    //{
                    //    logger.Info("Calling Hourly Xml for Source (" + source.Url + ")");
                    //    var manualChannelList = source.ActiveChannels.Where(c => c.IsManuallyAdded && c.IsActive).ToList();
                       
                    //    if (manualChannelList != null)
                    //    {
                    //        foreach (var channel in manualChannelList)
                    //        {
                    //            var sourcePath = ConfigurationManager.AppSettings["OutputFolderPath"] +
                    //                             channel.ChannelName + ".xml";

                    //            if (File.Exists(sourcePath))
                    //            {
                    //                logger.Info("Calling Process Hourly Xml for (" + Path.GetFileName(sourcePath) + ")");

                    //                var doc = XDocument.Load(sourcePath);

                    //                logger.Info("Completed Process Hourly Xml for (" + Path.GetFileName(sourcePath) + ")");
                    //            }
                    //        }
                    //    }

                    //    logger.Info("Completed Hourly Xml for Source (" + source.Url + ")");
                    //}
                    //catch (Exception)
                    //{
                         
                    //}
                    
                    #endregion Hourly Timetable Xml...
                }
                 
            }

            Console.WriteLine("Xml Generation Completed");

            if (OutputXmlList != null)
            {
                if (OutputXmlList.Count > 0)
                {
                    logger.Info("Updating Database...");
                    Console.WriteLine("Updating Database");  
                    
                    foreach (var outFile in OutputXmlList)
                    {
                        var fileDetails = outFile.Split(',');
                        var generatedId = String.Empty;
                        try
                        {
                            using (var dataContext = new EPGDataModel())
                            {
                                var fileName = Path.GetFileName(fileDetails[0]);
                                var dbEntry =
                                    dataContext.XmlImports.FirstOrDefault(x => x.XmlFileName.Trim().ToLower().Equals(fileName.Trim().ToLower()));

                                try
                                {
                                    if (dbEntry != null)
                                    {
                                        if(!string.IsNullOrEmpty(dbEntry.ChannelId))
                                            generatedId = dbEntry.ChannelId;
                                        else
                                        {
                                            generatedId = GetChannelId(Path.GetFileName(fileDetails[0]));
                                        }

                                        dataContext.XmlImports.Remove(dbEntry);
                                        dataContext.SaveChanges();
                                        
                                    }
                                    else
                                    {
                                        generatedId = GetChannelId(Path.GetFileName(fileDetails[0]));
                                    }
                                }
                                catch (Exception deleteException)
                                {
                                    logger.Error("Error occured while Deleting. Error Message - " +
                                        deleteException.Message);

                                    if (deleteException.InnerException != null)
                                        logger.Error("Error InnerException - " +
                                                 deleteException.InnerException.Message);
                                }
                                

                                try
                                {
                                    var xmlImport = new XmlImport
                                    {
                                        Url = "../Output/" + Path.GetFileName(fileDetails[0]),
                                        EpgStartDt = Convert.ToDateTime(fileDetails[1]),
                                        EpgEndDt = Convert.ToDateTime(fileDetails[2]),
                                        ImportDate = DateTime.Now,
                                        XmlFileName = Path.GetFileName(fileDetails[0]),
                                        Url2 = !string.IsNullOrEmpty(fileDetails[3]) ? fileDetails[3] : "",
                                        SourceUrl = !string.IsNullOrEmpty(fileDetails[4]) ? fileDetails[4] : "",
                                        ChannelId = generatedId
                                    };

                                    dataContext.XmlImports.Add(xmlImport);
                                    dataContext.SaveChanges();
                                }
                                catch (Exception xmlDataImportException)
                                {
                                    logger.Error("Error occured while adding new entry to Database. Error Message - " +
                                        xmlDataImportException.Message);

                                    if (xmlDataImportException.InnerException != null)
                                        logger.Error("Error InnerException - " +
                                                 xmlDataImportException.InnerException.Message);
                                  
                                }
                                
                            }
                        }
                        catch (Exception updateException)
                        {
                            logger.Error("Error occured while running UpdatE Database function. Error Message - " +
                            updateException.Message);

                            if (updateException.InnerException != null)
                                logger.Error("Error InnerException - " +
                                         updateException.InnerException.Message);
                        }

                    }

                    Console.WriteLine("Done Updating Database");
                    logger.Info("Done Updating Database");
                }
            }
        }

        public static void GenerateTimeTable()
        {
            TimetableXmlList.Clear();

            using (var dataContext = new EPGDataModel())
            {
                var channelList = dataContext.ActiveChannels.Where(c => c.IsManuallyAdded && c.IsActive).ToList();

                #region Create Xml...

                if (channelList.Count>0)
                {
                    try
                    {
                        foreach (var channel in channelList)
                        {
                            TimetableXmlList.AddRange(GenerationEngine.GenerateTimeTableXml(channel));
                        }
                    }
                    catch (Exception exception)
                    {}
                }

                #endregion Create Xml...

                #region Update Database...

                foreach (var entry in TimetableXmlList)
                {
                    var fileDetails = entry.Split(',');
                    var fileName = Path.GetFileName(fileDetails[0]);
                    if (fileName != null)
                    {
                        var generatedId = string.Empty;
                        var xmlFile = fileName.ToLower();
                        var dbEntry = dataContext.XmlImports.FirstOrDefault(c => c.XmlFileName.ToLower().Equals(xmlFile));
                        if (dbEntry != null)
                        {
                            if (!string.IsNullOrEmpty(dbEntry.ChannelId))
                                generatedId = dbEntry.ChannelId;
                            else
                            {
                                generatedId = GetChannelId(Path.GetFileName(xmlFile));
                            }

                            dataContext.XmlImports.Remove(dbEntry);
                            dataContext.SaveChanges();

                        }
                        else
                        {
                            generatedId = GetChannelId(Path.GetFileName(xmlFile));
                        }

                        try
                        {
                            var xmlImport = new XmlImport
                            {
                                Url = "../Output/" + Path.GetFileName(fileName),
                                EpgStartDt = Convert.ToDateTime(fileDetails[1]),
                                EpgEndDt = Convert.ToDateTime(fileDetails[2]),
                                ImportDate = DateTime.Now,
                                XmlFileName = Path.GetFileName(fileDetails[0]),
                                Url2 =  string.Empty,
                                SourceUrl = "Custom Channel",
                                ChannelId = generatedId
                            };

                            dataContext.XmlImports.Add(xmlImport);
                            dataContext.SaveChanges();
                        }
                        catch (Exception xmlDataImportException)
                        {
                            logger.Error("Error occured while adding new entry to Database. Error Message - " +
                                xmlDataImportException.Message);

                            if (xmlDataImportException.InnerException != null)
                                logger.Error("Error InnerException - " +
                                            xmlDataImportException.InnerException.Message);
                                  
                        }
                    }
                }

                #endregion Update Database...
            }

        }

        #region Functions...

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

                logger.Info("// Entered ExtractFileToDirectory.  Zip File Name IS -" + zipFileName + " & output directory is - " + outputDirectory);

                zip.ExtractAll(outputDirectory, ExtractExistingFileAction.OverwriteSilently);

                var hasFolder = zip.Any(entry => entry.FileName.Contains("/"));

                var files = Directory.GetFiles(outputDirectory + (hasFolder ? Path.GetFileNameWithoutExtension(zipFileName) : ""), "*.xml", SearchOption.TopDirectoryOnly);
              
                //var files = Directory.GetFiles(outputDirectory + Path.GetFileNameWithoutExtension(zipFileName), "*.xml", SearchOption.TopDirectoryOnly);

                foreach (var xmlFile in files)
                {
                    SourceXmlList.Add(xmlFile);
                }
            }
            catch (Exception zipExTractionException)
            {
                logger.Error("Error occured while running ExtractFileToDirectory function. Error Message - " +
                             zipExTractionException.Message);

                if(zipExTractionException.InnerException !=null)
                    logger.Error("Error InnerException - " +
                             zipExTractionException.InnerException.Message);
            } 
        }

        /// <summary>
        /// Clean destination directory for new set of source xmls.
        /// </summary>
        /// <param name="path">Path where xmls will be extracted.</param>
        private static void ClearDirectory(string path)
        {
            logger.Info("// Running function ClearDirectory.");
            foreach (var file in Directory.GetFiles(path).Where(file => !string.IsNullOrEmpty(file)))
            {
                File.Delete(file);
            }

            logger.Info("// Function ClearDirectory Completed.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceXml"></param>
        /// <param name="newChannelName"></param>
        /// <param name="newOffset"></param>
        [Obsolete]
        private static void GenerateOutputXml(string sourceXml, string newChannelName = null, int newOffset = 0)
        {
            Console.WriteLine("GenerateOutputXml > Source-"+sourceXml + ", Channel Name-"+newChannelName +", Offset-"+newOffset);

            var document = XDocument.Load(sourceXml);

            var _tv = new tv();
            var channelNode1 = document.Descendants("channel");

            var xElements = channelNode1 as XElement[] ?? channelNode1.ToArray();

            Console.WriteLine("Total Channels in current File - " + xElements.Length);

            for (var i = 0; i <= xElements.Length - 1; i++)
            {
                var elementChannel = xElements[i];
                var elementchannelId = elementChannel.Attributes("id").First().Value;


                var progArray = document.Descendants("programme")
                    .Where(x => (string)x.Attribute("channel").Value == elementchannelId);

                #region Channel...
                string id = string.Empty;
                string channelName = string.Empty;
                const string formatString = "yyyyMMddHHmmss";
                var outputFileName = string.Empty;

                #region Channel Name...

                if (!string.IsNullOrEmpty(newChannelName))
                {
                     channelName = newChannelName;
                    try
                    {
                        id = xElements[i].Attributes("id").First().Value;
                        outputFileName = xElements[i].Value;

                        if (string.IsNullOrEmpty(channelName))
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

                Console.WriteLine("Processing Channel - " + channelName);

                #endregion Channel Name...

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


                tvProgramme[] _tvProgrammes = new tvProgramme[sourceXElements.Count];

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
                        title = item.Element("title") != null ? item.Element("title").Value : "",
                        category = item.Element("category") != null ? item.Element("category").Value : "",
                        desc = item.Element("desc") != null ? item.Element("desc").Value : "",
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
                    _startDate = DateTime.ParseExact(startDt, formatString, CultureInfo.InvariantCulture);
                    _stopDate = DateTime.ParseExact(stopDt, formatString, CultureInfo.InvariantCulture);
                }
                catch (Exception generateException)
                {
                    logger.Error("Error occured while processing xml - " + sourceXml +"| Channel name-"+channelName+ " | Error message-"+generateException.Message);
                    if (generateException.InnerException != null)
                        logger.Error("Function Generate Xml Inner exception - " +
                                     generateException.InnerException.Message);
                }

                #endregion Programme Nodes...



                //2.> Nov-25 - Task ID:  1647 (duplicate channel bug ).
                if (outputFileName.Contains("http://"))
                {
                    outputFileName = outputFileName.Substring(0, outputFileName.IndexOf("http:", StringComparison.Ordinal));
                }
                //Nov-19 - Remove any Illegeal characters from filename.
                outputFileName = GetSafeFilename(outputFileName);

                _tv.Save(OutputFolderPath + outputFileName + ".xml");
                logger.Info("Generate Output xml -" + outputFileName + ".xml");


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
                        title = item.Element("title") != null ? item.Element("title").Value : "",
                        desc = item.Element("desc") != null ? item.Element("desc").Value : "",
                        subTitle = item.Element("sub-title") != null ? item.Element("sub-title").Value : "",
                        date = DateTime.Today,
                        originid = "",//Find
                        reserve = "no"
                    };

                    iNodeCount++;
                }

                _tv2.programme = _tvProgrammes2;

                _tv2.Save(OutputFolderPath + outputFileName + "_additional" + ".xml");

                #endregion Generate Additional Xml...

                OutputXmlList.Add(OutputFolderPath + outputFileName + ".xml" + "," + _startDate.Date.ToShortDateString() + "," + _stopDate.Date.ToShortDateString() + "," + "../Output/" + outputFileName + "_additional" + ".xml," + "../SourceXmlFiles/" + Path.GetFileName(sourceXml));

                Console.WriteLine("Done Processing Channel - " + channelName);
            }

        }

        /// <summary>
        /// Add Time Offset to Start & End attributes of output xml based on query string value for offset.
        /// </summary>
        /// <param name="originalTimeSpan">Original Start/ Stop value.</param>
        /// <param name="newOffset">Hours Offset to be added to original timespan.</param>
        /// <returns>String representation of original DateTime value after adding hours offset.</returns>
        private static string GetDateAddingOffset(string originalTimeSpan, int newOffset)
        {
            var dtAdditionalValue = string.Empty;
            string retval = string.Empty;

            try
            {
                if (originalTimeSpan.Contains("+"))
                {
                    dtAdditionalValue = originalTimeSpan.Substring(originalTimeSpan.IndexOf('+'));
                    originalTimeSpan = originalTimeSpan.Substring(0, originalTimeSpan.IndexOf('+')).Trim();
                }

                var originalDate = DateTime.ParseExact(originalTimeSpan, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);

                var newDate = originalDate.AddHours(newOffset);

                retval = newDate.Year.ToString() + newDate.Month.ToString() + newDate.Day.ToString("00").ToString() +
                               newDate.Hour.ToString("00") + newDate.Minute.ToString("00") + newDate.Second.ToString("00");
            }
            catch (Exception getDateAddingOffsetException)
            {
                logger.Error("Error occured - GetDateAddingOffset | Error message-"+getDateAddingOffsetException.Message);
                if (getDateAddingOffsetException.InnerException != null)
                    logger.Error("Inner Exception-" + getDateAddingOffsetException.InnerException.Message);
            }
            

            return retval + " " + dtAdditionalValue;
        }

        private static string DownloadFile(string sourceUrl, string extension)
        {
            sourceUrl = sourceUrl.Trim();
            var file = GetSafeFilename(Path.GetFileNameWithoutExtension(sourceUrl));
            var saveLocation = ZipFolderPath + file + extension;
              
            using (WebClient webClt = new WebClient())
            {
                try
                {
                    webClt.DownloadFile(sourceUrl, saveLocation);
                }
                catch (Exception downloadFileException)
                {
                    logger.Error("Error occured - DownloadFile | Error message-" + downloadFileException.Message);
                    if (downloadFileException.InnerException != null)
                        logger.Error("Inner Exception-" + downloadFileException.InnerException.Message);

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
        /// <param name="channel"></param>
        /// <returns></returns>
        private static string GetChannelId(string channel)
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

        #endregion Functions...
    }
}
