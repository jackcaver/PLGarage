using System.Collections.Generic;
using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    public class achievement
    {
        [XmlAttribute]
        public int achievement_type_id { get; set; }
        [XmlAttribute]
        public string created_at { get; set; }
        [XmlAttribute]
        public bool has_read { get; set; }
        [XmlAttribute]
        public int id { get; set; }
        [XmlAttribute]
        public int player_creation_id { get; set; }
        [XmlAttribute]
        public string player_creation_name { get; set; }
        [XmlAttribute]
        public int player_id { get; set; }
        [XmlAttribute]
        public string updated_at { get; set; }
    }

    public class achievements
    {
        [XmlAttribute]
        public int total { get; set; }
        [XmlElement("achievement")]
        public List<achievement> AchievementList { get; set; }
    }
}