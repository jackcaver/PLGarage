using System.Collections.Generic;
using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    [XmlType(TypeName = "tag")]
    public class Tag 
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

    [XmlType(TypeName = "tags")]
    public class Tags
    {
        [XmlAttribute]
        public string language { get; set; }
        [XmlAttribute]
        public string language_code { get; set; }
        [XmlElement("tag")]
        public List<Tag> TagList { get; set; }
    }
}