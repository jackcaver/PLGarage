using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    public class policy
    {
        [XmlAttribute]
        public int id { get; set; }
        [XmlAttribute]
        public bool is_accepted { get; set; }
        [XmlAttribute]
        public string name { get; set; }

        [XmlText]
        public string text { get; set; }
    }
}