using System.Collections.Generic;
using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    [XmlType(TypeName = "award")]    // TODO: Can we change this to XmlRoot?
    public class POIAward
    {
        [XmlAttribute("award_hash")]
        public string AwardHash { get; set; }
        [XmlAttribute("award_type")]
        public string AwardType { get; set; }
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlAttribute("required_checkins")]
        public int RequiredCheckins { get; set; }
        [XmlAttribute("locked")]
        public bool Locked { get; set; }
        [XmlAttribute("new_unlock")]
        public bool NewUnlock { get; set; }
    }

    [XmlType(TypeName = "poi")]
    public class POIResponse
    {
        [XmlElement("awards")]
        public List<POIAward> Awards { get; set; }
    }
}
