
#region Using ...

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using Ionic.Zip;
using XmlParser.Code;
using XmlParser.DataContext;

#endregion Using ...

namespace XmlParser
{
    public partial class Index : System.Web.UI.Page
    {
        #region Variables...

        readonly List<string> _sourceXmlList = new List<string>();
        readonly List<string> _outputXmlList = new List<string>();
        readonly List<string> _additionalOutputXmlList = new List<string>();

        DateTime _startDate = new DateTime();
        DateTime _stopDate = new DateTime();
        #endregion Variables...

        #region Events....

        protected void Page_Load(object sender, EventArgs e)
        {
           if(Session["USER_KEY"]==null)
               Response.Redirect("../default.aspx?id=ua");
           else
           {
               lblUser.Text = "Welcome " + Convert.ToString(Session["USER_KEY"]);
               if (Convert.ToString(Session["USR_TYPE"]) != "Admin")
                   hypHome.Visible = false;
           }

            if (!IsPostBack)
            {
                BindGrid();
                _sourceXmlList.Clear();
                _outputXmlList.Clear();
                _additionalOutputXmlList.Clear();
            }
        }

        protected void btnGetenerate_OnClick(object sender, EventArgs e)
        {
            _sourceXmlList.Clear();
            _outputXmlList.Clear();
            _additionalOutputXmlList.Clear();
            lblMsg.Text = "";
            grid.InnerHtml = "";

            foreach (GridViewRow row in gvXMLSource.Rows)
            {
                var chk = (row.FindControl("chkSelect") as CheckBox);
                if (chk != null && chk.Checked)
                {
                    var srno = Convert.ToInt32(gvXMLSource.DataKeys[row.RowIndex].Values[0]);

                    using (var dataContext =new EPGDataModel())
                    {
                        var source = dataContext.SourceURLs.Find(srno);
                        if (source != null)
                        {
                            var extension = source.Type.ToLower().Equals("zip") ? ".zip" : ".xml";
                            var fileName = DownloadFile(source.URL, extension);
                            //MODIFY HERE
                            if (!string.IsNullOrEmpty(fileName))
                            {
                                if (Path.GetExtension(fileName).Contains(".zip")) //Zip Archive.
                                {
                                    //extract xmls from zip archive.
                                    ExtractFileToDirectory(fileName, Server.MapPath(@"~/SourceXmlFiles/"));
                                }
                                else if (Path.GetExtension(fileName).Contains(".xml")) //Single Xml.
                                {
                                    //ClearDirectory(Server.MapPath(@"~/SourceXmlFiles/"));

                                    if (File.Exists(Server.MapPath(@"~/SourceXmlFiles/" + Path.GetFileName(fileName))))
                                        File.Delete(Server.MapPath(@"~/SourceXmlFiles/" + Path.GetFileName(fileName)));


                                    File.Copy(Server.MapPath(fileName),
                                        Server.MapPath(@"~/SourceXmlFiles/" + Path.GetFileName(fileName)));

                                    _sourceXmlList.Add(Server.MapPath(@"~/SourceXmlFiles/" + Path.GetFileName(fileName)));
                                }

                                 foreach (var xmlFile in _sourceXmlList)
                                 {

                                     var sourceUri = new Uri(source.URL);
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

                                     GenerateOutputXml(xmlFile,source.Srno, newChannelName, newOffset);
                                 }
                            }
                        }
                    }
                }

                _sourceXmlList.Clear();
            }



            if (_outputXmlList != null)
            {
                if (_outputXmlList.Count > 0)
                { 
                    grid.InnerHtml += "<span style='text-align:center;' class='alert-danger'>Generated Output Xml(s)</span>";
                    grid.InnerHtml += "<table style='width:50%;' class='table table-striped table-bordered table-hover table-condensed table-mptt'><th class='sortable'>Srno</th><th class='sortable'>Output Xml (click to view xml)</th><th class='sortable'>EPG Start Date</th><th class='sortable'>EPG End Date</th><tbody>";

                    var iCount = 1;
                    foreach (var outFile in _outputXmlList)
                    {
                        var fileDetails = outFile.Split(',');
                        grid.InnerHtml += "<tr><td>" + iCount + "</td><td><a href='" + fileDetails[0] + "' target='_blank' class='btn-link'> " + Path.GetFileName(fileDetails[0]) + "</a></td>";
                        grid.InnerHtml += "<td>" + fileDetails[1] + "</td><td>" + fileDetails[2] +"</td></tr>";


                        try
                        {
                            using (var dataContext = new EPGDataModel())
                            {
                                var fileName = Path.GetFileName(fileDetails[0]);
                                var dbEntry =
                                    dataContext.XmlImports.Where(
                                        x => x.XmlFileName.Trim().ToLower().Equals(fileName.Trim().ToLower())).ToList();

                                if (dbEntry != null)
                                {
                                    if (dbEntry.Count > 0)
                                    {
                                        dataContext.Database.ExecuteSqlCommand(
                                            "DELETE FROM XmlImport WHERE XmlFileName='" + fileName + "'");
                                    }
                                }

                                var xmlImport = new XmlImport
                                {
                                    Url = "../Output/" + Path.GetFileName(fileDetails[0]),
                                    EpgStartDt = _startDate,
                                    EpgEndDt = _stopDate,
                                    ImportDate = DateTime.Now,
                                    XmlFileName = Path.GetFileName(fileDetails[0]),
                                    Url2 = !string.IsNullOrEmpty(fileDetails[3])?fileDetails[3]:"",
                                    SourceUrl = !string.IsNullOrEmpty(fileDetails[4]) ? fileDetails[4] : "",
                                };

                                dataContext.XmlImports.Add(xmlImport);
                                dataContext.SaveChanges();
                            }
                        }
                        catch (Exception)
                        {

                        }

                        
                        iCount++;
                    }

                    
                    grid.InnerHtml += "</tbody></table>";
                    lblMsg.Text = "Xml(s) generated successfully!";

                }
            }
        }

        protected void btnAdd_OnClick(object sender, EventArgs e)
        {
             grid.InnerHtml = "";
            lblMsg.Text = "";
            using (var dataContext = new EPGDataModel())
            {
                var source = new SourceURL
                {
                    URL = txtSourceURLAdd.Text,
                    Type=ddlSourceType.SelectedValue,
                    IsActive = true,
                    EntryDate = DateTime.Today,
                    EntryId = Convert.ToString(Session["USER_KEY"]),
                    EntryIP = CommonFunctions.GetIpAddress()
                };
                dataContext.SourceURLs.Add(source);
                dataContext.SaveChanges();
                source.ActiveChannels = GetActiveChannels(source.URL, source.Srno);
                dataContext.SaveChanges(); 
            }

            lblMsg.Text = "Record added successfully!";
            BindGrid();
        }
         
        protected void btnImport_OnClick(object sender, EventArgs e)
        {
            if (fileUploadCtl.HasFile)
            {
                var fileName = Path.GetFileName(fileUploadCtl.FileName);

                fileUploadCtl.PostedFile.SaveAs(Server.MapPath(@"../ImportedList/"+fileName));

                var lines = File.ReadAllText(Server.MapPath(@"../ImportedList/" + fileName));

                var listOfUrls = lines.Split(',');

                foreach (var url in listOfUrls)
                {
                    string type = string.Empty;

                    switch (url.Substring(url.LastIndexOf('.'), 4).ToLower())
                    {
                        case ".zip":
                            type = "Zip";
                            break;
                        case ".xml":
                            type = "Xml";
                            break;
                    }

                    var sourceUrl = new SourceURL
                    {
                        URL = url,
                        Type = type,
                        EntryIP= CommonFunctions.GetIpAddress(),
                        EntryDate = DateTime.Today,
                        IsActive= true,
                        EntryId = Convert.ToString(Session["USER_KEY"]),
                    };
                     
                    try
                    {
                        using (var dataContext = new EPGDataModel())
                        {
                            dataContext.SourceURLs.Add(sourceUrl);
                            dataContext.SaveChanges();
                            sourceUrl.ActiveChannels = GetActiveChannels(sourceUrl.URL,sourceUrl.Srno);
                            dataContext.SaveChanges();
                        }
                    }
                    catch (Exception ex)
                    {
                        lblMsg.Text = "One or more Urls in text files already exsists in database.";
                    }

                }
                lblMsg.Text = "List Imported successfully!";
                BindGrid();
            }
        }

        private static ICollection<ActiveChannel> GetActiveChannels(string url, long id)
        {
            ICollection<ActiveChannel> activeChannelList=new List<ActiveChannel>();
            var type = string.Empty;

            switch (url.Substring(url.LastIndexOf('.'), 4).ToLower())
            {
                case ".zip":
                    type = "Zip";
                    break;
                case ".xml":
                    type = "Xml";
                    break;
            }

            if (type.Equals("Xml"))
            {
                var localFile = HttpContext.Current.Server.MapPath(DownloadFile(url, ".xml"));
                var doc = XDocument.Load(localFile);
                var channelList = doc.Descendants("channel").ToList();

                foreach (var channel in channelList)
                {
                    var name = channel.Value;

                    if (string.IsNullOrEmpty(name))
                        name = channel.Attributes("display-name").First().Value;

                    if (name.Contains("http://"))
                        name = name.Substring(0, name.IndexOf("http:", StringComparison.Ordinal));
                    
                    activeChannelList.Add(
                        new ActiveChannel
                        {
                            SourceId = id,
                            ChannelName = name,
                            IsActive = false
                        });
                }
            }
            else if (type.Equals("Zip"))
            {
                ClearDirectory(HttpContext.Current.Server.MapPath(@"../ZipArchives/"));
                var localFile = HttpContext.Current.Server.MapPath(DownloadFile(url, ".zip"));
                var zip = ZipFile.Read(localFile);
                zip.ExtractAll(HttpContext.Current.Server.MapPath(@"../ZipArchives/"), ExtractExistingFileAction.OverwriteSilently);

                var files = Directory.GetFiles(HttpContext.Current.Server.MapPath(@"../ZipArchives/" + Path.GetFileNameWithoutExtension(localFile)), "*.xml", SearchOption.TopDirectoryOnly);

                foreach (var xmlFile in files)
                {
                    var doc = XDocument.Load(xmlFile);
                    var channelList = doc.Descendants("channel").ToList();

                    foreach (var channel in channelList)
                    {
                        var name = channel.Value;

                        if (string.IsNullOrEmpty(name))
                            name = channel.Attributes("display-name").First().Value;

                        if (name.Contains("http://"))
                            name = name.Substring(0, name.IndexOf("http:", StringComparison.Ordinal));

                        activeChannelList.Add(
                            new ActiveChannel
                            {
                                SourceId = id,
                                ChannelName = name,
                                IsActive = false
                            });
                    }
                }
                 
            }

            return activeChannelList;
        }

        #endregion Events....

        #region Functions...

        /// <summary>
        /// 
        /// </summary>
        /// <param name="zipFileName"></param>
        /// <param name="outputDirectory"></param>
        private void ExtractFileToDirectory(string zipFileName, string outputDirectory)
        {
            //ClearDirectory(outputDirectory);

            var zip = ZipFile.Read(Server.MapPath(zipFileName));
            zip.ExtractAll(outputDirectory, ExtractExistingFileAction.OverwriteSilently);

            var files = Directory.GetFiles(outputDirectory, "*.xml", SearchOption.TopDirectoryOnly);

            foreach (var xmlFile in files)
            {
                _sourceXmlList.Add(xmlFile);
            }

        }

        /// <summary>
        /// Clean destination directory for new set of source xmls.
        /// </summary>
        /// <param name="path">Path where xmls will be extracted.</param>
        private static void ClearDirectory(string path)
        {
            foreach (var file in Directory.GetFiles(path).Where(file => !string.IsNullOrEmpty(file)))
            {
                File.Delete(file);
            }
        }

        /// <summary>
        /// Generates main output xml.
        /// </summary>
        /// <param name="sourceXml"></param>
        /// <param name="newChannelName"></param>
        /// <param name="newOffset"></param>
        private  void GenerateOutputXml(string sourceXml,long srno, string newChannelName=null, int newOffset=0)
        {
            
            var document = XDocument.Load(sourceXml);
             
            var _tv = new tv();
            var channelNode1 = document.Descendants("channel");
            
            var xElements = channelNode1 as XElement[] ?? channelNode1.ToArray();
            for (var i=0;i<=xElements.Length-1;i++)
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

                #endregion Channel Name...

                //1.> Nov-19 - Remove any Illegeal characters from filename.
                
                
                //2.> Nov-25 - Task ID:  1647 (duplicate channel bug ).
                if (outputFileName.Contains("http://"))
                {
                    outputFileName = outputFileName.Substring(0, outputFileName.IndexOf("http://", StringComparison.Ordinal));
                }

                outputFileName = GetSafeFilename(outputFileName);

                //Check if current channe is active.
                using (var dataContext=new EPGDataModel())
                {
                    var channelList =
                        dataContext.SourceURLs.Find(srno)
                            .ActiveChannels.FirstOrDefault(c => c.ChannelName.Equals(outputFileName) && c.IsActive);

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
                            var  creditsArray = creditElement.Nodes().ToList();

                            tvProgrammeNames = new tvProgrammeName[creditsArray.Count];
                            

                            for (var x=0;x<= creditsArray.Count-1;x++)
                            {
                                XNode node = creditsArray[x];
                                XElement element = (XElement) node;

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
                catch (Exception)
                {
                }

                #endregion Programme Nodes...

                 
                _tv.Save(Server.MapPath("../Output/" + outputFileName + ".xml"));


                #region Generate Additional Xml...

                var _tv2 = new Code.SecondOutput.tv();
                Code.SecondOutput.tvProgramme[] _tvProgrammes2 = new Code.SecondOutput.tvProgramme[sourceXElements.Count];


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

                    _tvProgrammes2[iNodeCount] = new Code.SecondOutput.tvProgramme()
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
                 

              
                  
                _tv2.Save(Server.MapPath("../Output/" + outputFileName +"_additional"+ ".xml"));

                #endregion Generate Additional Xml...

                 
                 _outputXmlList.Add("../Output/" + outputFileName + ".xml" + "," 
                     + _startDate.Date.ToShortDateString() + "," 
                     + _stopDate.Date.ToShortDateString() + ","
                     + "../Output/" + outputFileName + "_additional" + ".xml" + ","
                     + "../SourceXmlFiles/"+ Path.GetFileName(sourceXml)
                     );
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

            if (originalTimeSpan.Contains("+"))
            {
                dtAdditionalValue = originalTimeSpan.Substring(originalTimeSpan.IndexOf('+'));
                originalTimeSpan = originalTimeSpan.Substring(0,originalTimeSpan.IndexOf('+')).Trim();
            }

            var originalDate = DateTime.ParseExact(originalTimeSpan, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);

            var newDate = originalDate.AddHours(newOffset);

            var retval = newDate.Year.ToString() + newDate.Month.ToString() + newDate.Day.ToString("00").ToString() +
                           newDate.Hour.ToString("00") + newDate.Minute.ToString("00") + newDate.Second.ToString("00");

            return retval + " " +dtAdditionalValue;
        }

        private static string DownloadFile(string sourceUrl, string extension)
        {
            sourceUrl=sourceUrl.Trim();
            var file = GetSafeFilename(Path.GetFileNameWithoutExtension(sourceUrl));
            var saveLocation = "..\\ZipArchives\\" + file + extension;

            saveLocation = HttpContext.Current.Server.MapPath(saveLocation);

            using (WebClient webClt = new WebClient())
            {
                try
                {
                    webClt.DownloadFile(sourceUrl, saveLocation);
                }
                catch (Exception)
                {}
            }

            return "../ZipArchives/" + file + extension;
        }

        private static string GetSafeFilename(string illegal)
        {
            var regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            var r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
            illegal = r.Replace(illegal, "");

            return illegal;
        }

        #endregion Functions...

        #region Grid Events....
        private void BindGrid()
        {
            using (var dataContext = new EPGDataModel())
            {
                gvXMLSource.DataSource = dataContext.SourceURLs.ToList();
                gvXMLSource.DataBind();
            }
        }

        protected void gvXMLSource_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow && e.Row.RowIndex != gvXMLSource.EditIndex)
            {
                (e.Row.Cells[3].Controls[2] as ImageButton).Attributes["onclick"] =
                    "if(!confirm('Do you want to delete the record?')){ return false; };";

                var hypChannel = (HyperLink)e.Row.FindControl("hypChannel");
                if (hypChannel != null)
                {
                    var srno = Convert.ToInt64(gvXMLSource.DataKeys[e.Row.RowIndex].Values[0]);
                    using (var dataContext=new EPGDataModel())
                    {
                        var channelList =
                            dataContext.ActiveChannels.Where(c => c.SourceId.Equals(srno) && c.IsActive).ToList();

                        if (channelList.Count > 0)
                        {
                            hypChannel.CssClass = "btn btn-success";
                            hypChannel.ToolTip = channelList.Count +" channels active!";
                        }
                    }
                    
                }

            }
        }

        protected void gvXMLSource_OnRowEditing(object sender, GridViewEditEventArgs e)
        {
            gvXMLSource.EditIndex = e.NewEditIndex;
            BindGrid();
        }

        protected void gvXMLSource_OnRowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvXMLSource.EditIndex = -1;
            BindGrid();
        }

        protected void gvXMLSource_OnRowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            var row = gvXMLSource.Rows[e.RowIndex];
            var srno = Convert.ToInt32(gvXMLSource.DataKeys[e.RowIndex].Values[0]);
            var sourceUrl = (row.FindControl("txtSourceURL") as TextBox).Text;
            var sourceType = (row.FindControl("ddlGridSourceType") as DropDownList).SelectedValue;
            

            using (var dataContext = new EPGDataModel())
            {
                var source = dataContext.SourceURLs.Find(srno);

                source.URL = sourceUrl;
                source.Type = sourceType;
                source.EntryDate = DateTime.Today;
                source.IsActive = true;
                try
                {
                    dataContext.Entry(source).State = EntityState.Modified;
                    dataContext.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var eve in ex.EntityValidationErrors)
                    {
                        Console.WriteLine(
                            "Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                    throw;
                }


            }
            gvXMLSource.EditIndex = -1;
            BindGrid();
        }

        protected void gvXMLSource_OnRowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var srno = Convert.ToInt64(gvXMLSource.DataKeys[e.RowIndex].Values[0]);
            using (var dataContext = new EPGDataModel())
            {
                var source = dataContext.SourceURLs.Find(srno);
                dataContext.ActiveChannels.RemoveRange(source.ActiveChannels);
                dataContext.SourceURLs.Remove(source);
                dataContext.SaveChanges();
            }
            BindGrid();
        }

        #endregion Grid Events....
         
    }
}