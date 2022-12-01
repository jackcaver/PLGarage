using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    public class login_data
    {
        [XmlAttribute]
        public int player_id { get; set; }
        [XmlAttribute]
        public string player_name { get; set; }
        [XmlAttribute]
        public string presence { get; set; }
        [XmlAttribute]
        public string platform { get; set; }
        [XmlAttribute]
        public string login_time { get; set; }
        [XmlAttribute]
        public string ip_address { get; set; }
    }
}