using System.Collections.Generic;
using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    [XmlType(TypeName = "city")]
    public class City
    {
        [XmlAttribute]
        public string name { get; set; }
        [XmlAttribute]
        public int id { get; set; }
        [XmlAttribute]
        public float u { get; set; }
        [XmlAttribute]
        public float v { get; set; }
        [XmlAttribute]
        public float longitude { get; set; }
        [XmlAttribute]
        public float latitude { get; set; }
        [XmlAttribute]
        public bool has_new_unlocked { get; set; }
    }

    public class CitiesResponse
    {
        public List<City> cities { get; set; }
    }
}
