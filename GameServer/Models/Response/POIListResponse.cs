using System.Collections.Generic;
using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    [XmlType(TypeName = "poi")]    // TODO: Can we change this to XmlRoot?
    public class POI
    {
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlAttribute("id")]
        public int Id { get; set; }
        [XmlAttribute("u")]
        public float U { get; set; }
        [XmlAttribute("v")]
        public float V { get; set; }
        [XmlAttribute("longitude")]
        public float Longitude { get; set; }
        [XmlAttribute("latitude")]
        public float Latitude { get; set; }
        [XmlAttribute("locked")]
        public bool Locked { get; set; }
        [XmlAttribute("new_unlock")]
        public bool NewUnlock { get; set; }
        [XmlAttribute("global_checkin_count")]
        public int GlobalCheckinCount { get; set; }
    }

    public class POIListResponse
    {
        [XmlElement("points_of_interest")]
        public List<POI> PointsOfInterest { get; set; }
    }
}
