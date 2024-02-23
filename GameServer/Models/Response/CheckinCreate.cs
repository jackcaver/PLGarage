using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    [XmlType(TypeName = "checkin")]
    public class CheckinCreate
    {
        [XmlAttribute]
        public int id { get; set; }
        [XmlAttribute]
        public float last_miles { get; set; }
        [XmlAttribute]
        public float last_points { get; set; }
        [XmlAttribute]
        public float global_miles { get; set; }
        [XmlAttribute]
        public float global_points { get; set; }
        [XmlAttribute]
        public float travel_points { get; set; }
        [XmlAttribute]
        public float total_miles { get; set; }
        [XmlAttribute]
        public float u { get; set; }
        [XmlAttribute]
        public float v { get; set; }
        [XmlAttribute]
        public bool new_unlock { get; set; }
    }
}
