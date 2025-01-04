using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    [XmlType(TypeName = "checkin")]    // TODO: Can we change this to XmlRoot?
    public class CheckinCreate
    {
        [XmlAttribute("id")]
        public int Id { get; set; }
        [XmlAttribute("last_miles")]
        public float LastMiles { get; set; }
        [XmlAttribute("last_points")]
        public float LastPoints { get; set; }
        [XmlAttribute("global_miles")]
        public float GlobalMiles { get; set; }
        [XmlAttribute("global_points")]
        public float GlobalPoints { get; set; }
        [XmlAttribute("travel_points")]
        public float TravelPoints { get; set; }
        [XmlAttribute("total_miles")]
        public float TotalMiles { get; set; }
        [XmlAttribute("u")]
        public float U { get; set; }
        [XmlAttribute("v")]
        public float V { get; set; }
        [XmlAttribute("new_unlock")]
        public bool NewUnlock { get; set; }
    }
}
