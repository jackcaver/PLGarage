using System.Collections.Generic;
using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    public class SkillLevelPlayer
    {
        [XmlAttribute]
        public int id { get; set; }
        [XmlAttribute]
        public int skill_level_id { get; set; }
        [XmlAttribute]
        public string skill_level_name { get; set; }
        [XmlAttribute]
        public string username { get; set; }
    }

    [XmlType(TypeName = "players")]
    public class SkillLevelResponse
    {
        [XmlAttribute]
        public int total { get; set; }
        [XmlElement("player")]
        public List<SkillLevelPlayer> playersList { get; set; }
    }
}
