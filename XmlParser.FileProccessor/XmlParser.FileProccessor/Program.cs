using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlParser.FileProccessor
{
    class Program
    {
        static void Main(string[] args)
        {
           Processor.MoveXmlFile();
           Console.WriteLine("Completed Operation");
        }

        
    }
}
