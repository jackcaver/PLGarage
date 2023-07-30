using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    [XmlType(TypeName = "ticket")]
    public class TicketResponse
    {
        [XmlAttribute]
        public string expiration_date { get; set; }
        [XmlAttribute]
        public int player_id { get; set; }
        [XmlAttribute]
        public string session_uuid { get; set; }
        [XmlAttribute]
        public string signature { get; set; }
        [XmlAttribute]
        public string username { get; set; }
    }

    [XmlType(TypeName = "server")]
    public class ServerResponse
    {
        [XmlAttribute]
        public string address { get; set; }
        [XmlAttribute]
        public int port { get; set; }
        [XmlAttribute]
        public string server_private_key { get; set; }
        [XmlAttribute]
        public string server_type { get; set; }
        [XmlAttribute]
        public string session_uuid { get; set; }

        public TicketResponse ticket { get; set; }
    }
}