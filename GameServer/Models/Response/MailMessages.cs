using System.Collections.Generic;
using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    [XmlRoot("mail_message")]
    public class MailMessage
    {
        [XmlAttribute("attachment_reference")]
        public string AttachmentReference { get; set; }
        [XmlAttribute("created_at")]
        public string CreatedAt { get; set; }
        [XmlAttribute("has_deleted")]
        public bool HasDeleted { get; set; }
        [XmlAttribute("has_forwarded")]
        public bool HasForwarded { get; set; }
        [XmlAttribute("has_read")]
        public bool HasRead { get; set; }
        [XmlAttribute("has_replied")]
        public bool HasReplied { get; set; }
        [XmlAttribute("id")]
        public int Id { get; set; }
        [XmlAttribute("sender_id")]
        public int SenderId { get; set; }
        [XmlAttribute("sender_name")]
        public string SenderName { get; set; }
        [XmlAttribute("updated_at")]
        public string UpdatedAt { get; set; }
        [XmlElement("subject")]
        public string Subject { get; set; }
        [XmlElement("body")]
        public string Body { get; set; }
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
