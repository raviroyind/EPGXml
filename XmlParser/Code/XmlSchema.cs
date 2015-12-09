using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

// ReSharper disable once InconsistentNaming
namespace XmlParser.Code
{
    
        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "tv:noNamespaceSchemaLocation", IsNullable = false)]
        public class tv
        {

            public void Save(string fileName)
            {
                using (var writer = new System.IO.StreamWriter(fileName))
                {
                    var serializer = new XmlSerializer(this.GetType());
                    serializer.Serialize(writer, this);
                    writer.Flush();
                }
            }

            private tvChannel channelField;

            private tvProgramme[] programmeField;

            /// <remarks/>
            public tvChannel channel
            {
                get
                {
                    return this.channelField;
                }
                set
                {
                    this.channelField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute("programme")]
            public tvProgramme[] programme
            {
                get
                {
                    return this.programmeField;
                }
                set
                {
                    this.programmeField = value;
                }
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class tvChannel
        {

            private string displaynameField;

            private string idField;

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute("display-name")]
            public string displayname
            {
                get
                {
                    return this.displaynameField;
                }
                set
                {
                    this.displaynameField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string id
            {
                get
                {
                    return this.idField;
                }
                set
                {
                    this.idField = value;
                }
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class tvProgramme
        {
            private string channelField;

            private tvProgrammeLength lengthField;

            private string titleField;

            private string categoryField;

            private string descField;

            private tvProgrammeName[] creditsField;

            private string imageField;

            private string startField;

            private string stopField;

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string channel
            {
                get
                {
                    return this.channelField;
                }
                set
                {
                    this.channelField = value;
                }
            }

            /// <remarks/>
            public tvProgrammeLength length
            {
                get
                {
                    return this.lengthField;
                }
                set
                {
                    this.lengthField = value;
                }
            }

            /// <remarks/>
            public string title
            {
                get
                {
                    return this.titleField;
                }
                set
                {
                    this.titleField = value;
                }
            }

            /// <remarks/>
            public string category
            {
                get
                {
                    return this.categoryField;
                }
                set
                {
                    this.categoryField = value;
                }
            }

            /// <remarks/>
            public string desc
            {
                get
                {
                    return this.descField;
                }
                set
                {
                    this.descField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlArrayItemAttribute("name", IsNullable = false)]
            public tvProgrammeName[] credits
            {
                get
                {
                    return this.creditsField;
                }
                set
                {
                    this.creditsField = value;
                }
            }

            /// <remarks/>
            public string image
            {
                get
                {
                    return this.imageField;
                }
                set
                {
                    this.imageField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string start
            {
                get
                {
                    return this.startField;
                }
                set
                {
                    this.startField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string stop
            {
                get
                {
                    return this.stopField;
                }
                set
                {
                    this.stopField = value;
                }
            }

            
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class tvProgrammeLength
        {

            private string unitsField;

            private ushort valueField;

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string units
            {
                get
                {
                    return this.unitsField;
                }
                set
                {
                    this.unitsField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlTextAttribute()]
            public ushort Value
            {
                get
                {
                    return this.valueField;
                }
                set
                {
                    this.valueField = value;
                }
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class tvProgrammeName
        {

            private string functionField;

            private string valueField;

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string function
            {
                get
                {
                    return this.functionField;
                }
                set
                {
                    this.functionField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlTextAttribute()]
            public string Value
            {
                get
                {
                    return this.valueField;
                }
                set
                {
                    this.valueField = value;
                }
            }
        }
  
}
