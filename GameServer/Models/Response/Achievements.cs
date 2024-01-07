using System.Collections.Generic;
using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    [XmlType(TypeName = "achievement")]
    public class Achievement
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
        public string player_creation_id { get; set; }
        [XmlAttribute]
        public string player_creation_name { get; set; }
        [XmlAttribute]
        public int player_id { get; set; }
        [XmlAttribute]
        public string updated_at { get; set; }
        //MNR
        [XmlAttribute]
        public bool relevant { get; set; }
        [XmlAttribute]
        public string value { get; set; }
    }

    [XmlType(TypeName = "achievements")]
    public class Achievements
    {
        [XmlAttribute]
        public int total { get; set; }
        [XmlElement("achievement")]
        public List<Achievement> AchievementList { get; set; }
    }
}