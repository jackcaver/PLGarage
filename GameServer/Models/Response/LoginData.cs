using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    [XmlRoot("login_data")]
    public class LoginData
    {
        [XmlAttribute("player_id")]
        public int PlayerId { get; set; }
        [XmlAttribute("player_name")]
        public string PlayerName { get; set; }
        [XmlAttribute("presence")]
        public string Presence { get; set; }
        [XmlAttribute("platform")]
        public string Platform { get; set; }
        [XmlAttribute("login_time")]
        public string LoginTime { get; set; }
        [XmlAttribute("ip_address")]
        public string IpAddress { get; set; }
    }
}