using System.Collections.Generic;
using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    public class tag 
    {
        
        [XmlAttribute]
        public string category { get; set; }
        [XmlAttribute]
        public string key { get; set; }
        [XmlAttribute]
        public string name { get; set; }
        [XmlAttribute]
        public string type { get; set; }
    }

    public class tags
    {
        [XmlAttribute]
        public string language { get; set; }
        [XmlAttribute]
        public string language_code { get; set; }
        [XmlElement("tag")]
        public List<tag> TagList { get; set; }
    }
}