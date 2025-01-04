using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    [XmlType(TypeName = "location")]    // TODO: Can we change this to XmlRoot?
    public class Location
    {
        [XmlAttribute("longitude")]
        public float Longitude { get; set; }
        [XmlAttribute("latitude")]
        public float Latitude { get; set; }
        [XmlAttribute("tag")]
        public string Tag { get; set; }
        [XmlAttribute("is_tagged")]
        public bool IsTagged { get; set; }
    }
}
