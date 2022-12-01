using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    public class player_profile 
    {
        [XmlAttribute]
        public int player_id { get; set; }
        [XmlAttribute]
        public string quote { get; set; }
        [XmlAttribute]
        public string username { get; set; }
    }
}