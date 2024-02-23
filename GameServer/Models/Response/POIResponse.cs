using System.Collections.Generic;
using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    [XmlType(TypeName = "award")]
    public class POIAward
    {
        [XmlAttribute]
        public string award_hash { get; set; }
        [XmlAttribute]
        public string award_type { get; set; }
        [XmlAttribute]
        public string name { get; set; }
        [XmlAttribute]
        public int required_checkins { get; set; }
        [XmlAttribute]
        public bool locked { get; set; }
        [XmlAttribute]
        public bool new_unlock { get; set; }
    }

    [XmlType(TypeName = "poi")]
    public class POIResponse
    {
        public List<POIAward> awards { get; set; }
    }
}
