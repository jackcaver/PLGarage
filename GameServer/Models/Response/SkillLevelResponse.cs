using System.Collections.Generic;
using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    public class SkillLevelPlayer
    {
        [XmlAttribute("id")]
        public int Id { get; set; }
        [XmlAttribute("skill_level_id")]
        public int SkillLevelId { get; set; }
        [XmlAttribute("skill_level_name")]
        public string SkillLevelName { get; set; }
        [XmlAttribute("username")]
        public string Username { get; set; }
    }

    [XmlType(TypeName = "players")]    // TODO: Can we change this to XmlRoot?
    public class SkillLevelResponse
    {
        [XmlAttribute("total")]
        public int Total { get; set; }
        [XmlElement("player")]
        public List<SkillLevelPlayer> PlayersList { get; set; }
    }
}
