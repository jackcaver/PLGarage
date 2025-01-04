using System.Collections.Generic;
using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    [XmlType(TypeName = "travel_award")]    // TODO: Can we change this to XmlRoot?
    public class TravelAward
    {
        [XmlAttribute("award_hash")]
        public string AwardHash { get; set; }
        [XmlAttribute("award_type")]
        public string AwardType { get; set; }
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlAttribute("individual_points")]
        public int IndividualPoints { get; set; }
        [XmlAttribute("global_points")]
        public int GlobalPoints { get; set; }
        [XmlAttribute("unlocked")]
        public bool Unlocked { get; set; }
        [XmlAttribute("new_unlock")]
        public bool NewUnlock { get; set; }
        [XmlAttribute("is_global_type")]
        public bool IsGlobalType { get; set; }
    }

    public class TravelAwardsResponse
    {
        [XmlElement("travel_awards")]
        public List<TravelAward> TravelAwards { get; set; }
    }
}
