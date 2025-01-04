using System.Collections.Generic;
using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    [XmlRoot("mail_message")]
    public class MailMessage    // TODO: Why are there 2 models here?
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

    public class mailMessage    // TODO: Why are there 2 models here?
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
        [XmlAttribute("mail_message_type")]
        public string MailMessageType { get; set; }
        [XmlAttribute("recipient_id")]
        public int RecipientId { get; set; }
        [XmlAttribute("recipient_list")]
        public string RecipientList { get; set; }
        [XmlAttribute("sender_id")]
        public int SenderId { get; set; }
        [XmlAttribute("sender_name")]
        public string SenderName { get; set; }
        [XmlAttribute("subject")]
        public string Subject { get; set; }
        [XmlAttribute("updated_at")]
        public string UpdatedAt { get; set; }
    }

    [XmlRoot("mail_messages")]
    public class MailMessages
    {
        [XmlAttribute("page")]
        public int Page { get; set; }
        [XmlAttribute("player_id")]
        public int PlayerId { get; set; }
        [XmlAttribute("row_end")]
        public int RowEnd { get; set; }
        [XmlAttribute("row_start")]
        public int RowStart { get; set; }
        [XmlAttribute("total")]
        public int Total { get; set; }
        [XmlAttribute("total_pages")]
        public int TotalPages { get; set; }
        [XmlAttribute("unread_count")]
        public int UnreadCount { get; set; }
        [XmlElement("mail_message")]
        public List<mailMessage> MailMessagesList { get; set; }
    }
}
