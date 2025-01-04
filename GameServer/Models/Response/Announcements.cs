using System.Collections.Generic;
using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    public class Announcement
    {
        [XmlAttribute("created_at")]
        public string CreatedAt { get; set; }
        [XmlAttribute("id")]
        public int Id { get; set; }
        [XmlAttribute("language_code")]
        public string LanguageCode { get; set; }
        [XmlAttribute("subject")]
        public string Subject { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlType(TypeName = "announcements")]    // TODO: Can we change this to XmlRoot?
    public class Announcements
    {
        [XmlAttribute("total")]
        public int Total { get; set; }
        [XmlElement("announcement")]
        public List<Announcement> AnnouncementList { get; set; }
    }
}
