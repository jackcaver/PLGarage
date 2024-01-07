using System.Collections.Generic;
using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    public class mail_message
    {
        [XmlAttribute]
        public string attachment_reference { get; set; }
        [XmlAttribute]
        public string created_at { get; set; }
        [XmlAttribute]
        public bool has_deleted { get; set; }
        [XmlAttribute]
        public bool has_forwarded { get; set; }
        [XmlAttribute]
        public bool has_read { get; set; }
        [XmlAttribute]
        public bool has_replied { get; set; }
        [XmlAttribute]
        public int id { get; set; }
        [XmlAttribute]
        public int sender_id { get; set; }
        [XmlAttribute]
        public string sender_name { get; set; }
        [XmlAttribute]
        public string updated_at { get; set; }
        public string subject { get; set; }
        public string body { get; set; }
    }

    public class mailMessage
    {
        [XmlAttribute]
        public string attachment_reference { get; set; }
        [XmlAttribute]
        public string created_at { get; set; }
        [XmlAttribute]
        public bool has_deleted { get; set; }
        [XmlAttribute]
        public bool has_forwarded { get; set; }
        [XmlAttribute]
        public bool has_read { get; set; }
        [XmlAttribute]
        public bool has_replied { get; set; }
        [XmlAttribute]
        public int id { get; set; }
        [XmlAttribute]
        public string mail_message_type { get; set; }
        [XmlAttribute]
        public int recipient_id { get; set; }
        [XmlAttribute]
        public string recipient_list { get; set; }
        [XmlAttribute]
        public int sender_id { get; set; }
        [XmlAttribute]
        public string sender_name { get; set; }
        [XmlAttribute]
        public string subject { get; set; }
        [XmlAttribute]
        public string updated_at { get; set; }
    }

    public class mail_messages
    {
        [XmlAttribute]
        public int page { get; set; }
        [XmlAttribute]
        public int player_id { get; set; }
        [XmlAttribute]
        public int row_end { get; set; }
        [XmlAttribute]
        public int row_start { get; set; }
        [XmlAttribute]
        public int total { get; set; }
        [XmlAttribute]
        public int total_pages { get; set; }
        [XmlAttribute]
        public int unread_count { get; set; }
        [XmlElement("mail_message")]
        public List<mailMessage> mailMessagesList { get; set; }
    }
}
