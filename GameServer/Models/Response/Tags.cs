using System.Collections.Generic;
using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    [XmlType(TypeName = "tag")]    // TODO: Can we change this to XmlRoot?
    public class Tag 
    {
        
        [XmlAttribute("category")]
        public string Category { get; set; }
        [XmlAttribute("key")]
        public string Key { get; set; }
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlAttribute("type")]
        public string Type { get; set; }
    }

    [XmlType(TypeName = "tags")]    // TODO: Can we change this to XmlRoot?
    public class Tags
    {
        [XmlAttribute("language")]
        public string Language { get; set; }
        [XmlAttribute("language_code")]
        public string LanguageCode { get; set; }
        [XmlElement("tag")]
        public List<Tag> TagList { get; set; }
    }
}