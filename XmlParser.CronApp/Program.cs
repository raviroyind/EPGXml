using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XmlGeneration.CronApp
{
    class Program
    {
        static void Main(string[] args)
        {
           XmlProcessor.ProcessXml();
           XmlProcessor.GenerateTimeTable();
        }
    }
}
