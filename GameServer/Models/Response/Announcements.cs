using System.Collections.Generic;
using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    public class Announcement
    {
        [XmlAttribute]
        public string created_at { get; set; }
        [XmlAttribute]
        public int id { get; set; }
        [XmlAttribute]
        public string language_code { get; set; }
        [XmlAttribute]
        public string subject { get; set; }
        [XmlText]
        public string text { get; set; }
    }

    [XmlType(TypeName = "announcements")]
    public class Announcements
    {
        [XmlAttribute]
        public int total { get; set; }
        [XmlElement("announcement")]
        public List<Announcement> AnnouncementList { get; set; }
    }
}
