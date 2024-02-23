using System.Collections.Generic;
using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    [XmlType(TypeName = "travel_award")]
    public class TravelAward
    {
        [XmlAttribute]
        public string award_hash { get; set; }
        [XmlAttribute]
        public string award_type { get; set; }
        [XmlAttribute]
        public string name { get; set; }
        [XmlAttribute]
        public int individual_points { get; set; }
        [XmlAttribute]
        public int global_points { get; set; }
        [XmlAttribute]
        public bool unlocked { get; set; }
        [XmlAttribute]
        public bool new_unlock { get; set; }
        [XmlAttribute]
        public bool is_global_type { get; set; }
    }

    public class TravelAwardsResponse
    {
        public List<TravelAward> travel_awards { get; set; }
    }
}
