using System.Collections.Generic;
using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    [XmlType(TypeName = "poi")]
    public class POI
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
        public bool locked { get; set; }
        [XmlAttribute]
        public bool new_unlock { get; set; }
        [XmlAttribute]
        public int global_checkin_count { get; set; }
    }

    public class POIListResponse
    {
        public List<POI> points_of_interest { get; set; }
    }
}
