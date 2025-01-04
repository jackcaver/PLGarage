using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    [XmlType(TypeName = "poi")]    // TODO: Can we change this to XmlRoot?
    public class CheckinStatus
    {
        [XmlAttribute("id")]
        public int Id { get; set; }
        [XmlAttribute("u")]
        public float U { get; set; }
        [XmlAttribute("v")]
        public float V { get; set; }
        [XmlAttribute("latitude")]
        public float Latitude { get; set; }
        [XmlAttribute("longitude")]
        public float Longitude { get; set; }
        [XmlAttribute("radius")]
        public float Radius { get; set; }
        [XmlAttribute("name")]
        public string Name { get; set; }
    }
}
