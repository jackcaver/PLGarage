using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    [XmlType(TypeName = "location")]
    public class Location
    {
        [XmlAttribute]
        public float longitude { get; set; }
        [XmlAttribute]
        public float latitude { get; set; }
        [XmlAttribute]
        public string tag { get; set; }
        [XmlAttribute]
        public bool is_tagged { get; set; }
    }
}
