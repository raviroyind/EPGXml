using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlParser.FileProccessor
{
    public static class Processor
    {
        public static HashSet<string> FileHashSet { get; private set; }

        public static HashSet<string> MoveXmlFile()
        {

            #region Past7 to Recycle...

            var dInfoRecycle = new DirectoryInfo(ConfigurationManager.AppSettings["PastXmlFolderPath"] + @"Past7\");
            var fInfoRecycle = dInfoRecycle.GetFiles("*.xml");
            var numberOfFiles  = fInfoRecycle.Count();
            for (var i = 0; i < numberOfFiles; i++)
            {
                Console.WriteLine("Moving : " + fInfoRecycle[i].Name);
                var result = fInfoRecycle[i].CopyTo(ConfigurationManager.AppSettings["PastXmlFolderPath"] +   @"\RecycleBin\" + fInfoRecycle[i].Name);
            }

            Console.WriteLine("Completed Past7 to Recycle");

            #endregion Past7 to Recycle...

            FileHashSet = new HashSet<string>();

            ClearDirectory(ConfigurationManager.AppSettings["PastXmlFolderPath"] + @"Past7\");

            for (var x = 7; x >= 1; x--)
            {
                var dInfoDay = new DirectoryInfo(ConfigurationManager.AppSettings["PastXmlFolderPath"] + "Day"+x);

                var fInfoDay = dInfoDay.GetFiles("*.xml");

                if (fInfoDay.Any())
                {
                    var numberOfFilesToCopy = fInfoDay.Count();
                    Console.WriteLine("Day " + x + " Total file count : " + numberOfFilesToCopy);

                    Console.WriteLine("Starting Day " + x + " Copy to RecycleBin");

                    for (var i = 0; i < Math.Max(fInfoDay.Length, numberOfFilesToCopy); i++)
                    {
                        Console.WriteLine("Moving : " + fInfoDay[i].Name);
                        var result = fInfoDay[i].CopyTo(ConfigurationManager.AppSettings["PastXmlFolderPath"] + (x == 7 ? @"Past7\" : @"Day"+(x+1)+@"\") + fInfoDay[i].Name);

                        if (!string.IsNullOrEmpty(result.Name))
                            File.Delete(fInfoDay[i].FullName);

                    }

                    FileHashSet.Add((from c in fInfoDay select c.Name).ToString());
                    dInfoDay = null;
                    fInfoDay = null;

                    Console.WriteLine("Completed Day  " + x + "  Copy to RecycleBin");
                }
            }


            #region System to Day 1...

            var dInfoOutput = new DirectoryInfo(ConfigurationManager.AppSettings["OutputFolderPath"]);

            var fInfodInfoOutput = dInfoOutput.GetFiles("*.xml");

            if (fInfodInfoOutput.Any())
            {
                var numberOfFilesToCopy = fInfodInfoOutput.Count();
                Console.WriteLine("Output Folder Total file count : " + numberOfFilesToCopy);

                Console.WriteLine("Starting Output Folder Copy to Day1 Folder");

                for (var i = 0; i < Math.Max(fInfodInfoOutput.Length, numberOfFilesToCopy); i++)
                {
                    Console.WriteLine("Moving : " + fInfodInfoOutput[i].Name);
                    var result = fInfodInfoOutput[i].CopyTo(ConfigurationManager.AppSettings["PastXmlFolderPath"] + @"Day1\" + fInfodInfoOutput[i].Name);
                     
                }

                FileHashSet.Add((from c in fInfodInfoOutput select c.Name).ToString());
                dInfoOutput = null;
                fInfodInfoOutput = null;

                Console.WriteLine("Completed Output Folder Copy to Day1 Folder.");
            }

            #endregion System to Day 1...

            return FileHashSet;
        }
         
        public static void MoveXmlFileWeekly()
        {
            #region System to Past7...

            var dInfoOutput = new DirectoryInfo(ConfigurationManager.AppSettings["OutputFolderPath"]);

            var fInfodInfoOutput = dInfoOutput.GetFiles("*.xml");

            if (fInfodInfoOutput.Any())
            {
                var numberOfFilesToCopy = fInfodInfoOutput.Count();
                Console.WriteLine("Output Folder Total file count : " + numberOfFilesToCopy);

                Console.WriteLine("Starting Output Folder Copy to Past7 Folder");

                for (var i = 0; i < Math.Max(fInfodInfoOutput.Length, numberOfFilesToCopy); i++)
                {
                    Console.WriteLine("Moving : " + fInfodInfoOutput[i].Name);
                    var result = fInfodInfoOutput[i].CopyTo(ConfigurationManager.AppSettings["PastXmlFolderPath"] + @"Past7\" + fInfodInfoOutput[i].Name);
                }

                Console.WriteLine("Completed Output Folder Copy to Past7 Folder.");
            }

            #endregion System to Past7...
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


    }
}
