using System.Collections.Generic;
using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    [XmlType(TypeName = "city")]    // TODO: Can we change this to XmlRoot?
    public class City
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
        [XmlAttribute("has_new_unlocked")]
        public bool HasNewUnlocked { get; set; }
    }

    public class CitiesResponse
    {
        [XmlElement("cities")]
        public List<City> Cities { get; set; }
    }
}
