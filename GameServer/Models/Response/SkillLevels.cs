using System.Collections.Generic;
using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    [XmlRoot("skill_level")]
    public class SkillLevel
    {
        [XmlAttribute("id")]
        public int Id { get; set; }
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlAttribute("points")]
        public int Points { get; set; }
    }

    [XmlRoot("skill_levels")]
    public class SkillLevels
    {
        [XmlAttribute("total")]
        public int Total { get; set; }
        [XmlElement("skill_level")]
        public List<SkillLevel> SkillLevelList { get; set; }
    }
}
