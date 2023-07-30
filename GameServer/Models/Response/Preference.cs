using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    [XmlType(TypeName = "preference")]
    public class Preference
    {
        [XmlAttribute]
        public string domain { get; set; }
        [XmlAttribute]
        public string ip_address { get; set; }
        [XmlAttribute]
        public string language_code { get; set; }
        [XmlAttribute]
        public string region_code { get; set; }
        [XmlAttribute]
        public string timezone { get; set; }
    }
}