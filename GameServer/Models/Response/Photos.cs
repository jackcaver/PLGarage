using System.Collections.Generic;
using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    [XmlType(TypeName = "photo")]
    public class Photo 
    {        
        [XmlAttribute]
        public string associated_usernames { get; set; }
        [XmlAttribute]
        public int id { get; set; }
        [XmlAttribute]
        public int track_id { get; set; }
        [XmlAttribute]
        public string username { get; set; }
    }

    [XmlType(TypeName = "photos")]
    public class Photos
    {
        [XmlAttribute]
        public int current_page { get; set; }
        [XmlAttribute]
        public int row_end { get; set; }
        [XmlAttribute]
        public int row_start { get; set; }
        [XmlAttribute]
        public int total { get; set; }
        [XmlAttribute]
        public int total_pages { get; set; }
        [XmlElement("photo")]
        public List<Photo> PhotoList { get; set; }
    }
}