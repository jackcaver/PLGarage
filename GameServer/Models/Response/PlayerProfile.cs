using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    [XmlRoot("player_profile")]
    public class PlayerProfile 
    {
        [XmlAttribute("player_id")]
        public int PlayerId { get; set; }
        [XmlAttribute("quote")]
        public string Quote { get; set; }
        [XmlAttribute("username")]
        public string Username { get; set; }
    }
}