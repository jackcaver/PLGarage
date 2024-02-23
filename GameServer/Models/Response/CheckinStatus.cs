using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    [XmlType(TypeName = "poi")]
    public class CheckinStatus
    {
        [XmlAttribute]
        public int id { get; set; }
        [XmlAttribute]
        public float u { get; set; }
        [XmlAttribute]
        public float v { get; set; }
        [XmlAttribute]
        public float latitude { get; set; }
        [XmlAttribute]
        public float longitude { get; set; }
        [XmlAttribute]
        public float radius { get; set; }
        [XmlAttribute]
        public string name { get; set; }
    }
}
