using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    [XmlType(TypeName = "ticket")]    // TODO: Can we change this to XmlRoot?
    public class TicketResponse
    {
        [XmlAttribute("expiration_date")]
        public string ExpirationDate { get; set; }
        [XmlAttribute("player_id")]
        public int PlayerId { get; set; }
        [XmlAttribute("session_uuid")]
        public string SessionUuid { get; set; }
        [XmlAttribute("signature")]
        public string Signature { get; set; }
        [XmlAttribute("username")]
        public string Username { get; set; }
    }

    [XmlType(TypeName = "server")]    // TODO: Can we change this to XmlRoot?
    public class ServerResponse
    {
        [XmlAttribute("address")]
        public string Address { get; set; }
        [XmlAttribute("port")]
        public int Port { get; set; }
        [XmlAttribute("server_private_key")]
        public string ServerPrivateKey { get; set; }
        [XmlAttribute("server_type")]
        public string ServerType { get; set; }
        [XmlAttribute("session_uuid")]
        public string SessionUuid { get; set; }

        [XmlElement("ticket")]
        public TicketResponse Ticket { get; set; }
    }
}