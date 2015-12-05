using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlParser.Core.Core
{

    public static  class ParserEngine
    {
        #region Variables...

       static readonly List<string> SourceXmlList = new List<string>();
       static readonly List<string> OutputXmlList = new List<string>();
        
       public static DateTime StartDate { get; set; }
       public static DateTime StopDate { get; set; }

      #endregion Variables...

    }
}
