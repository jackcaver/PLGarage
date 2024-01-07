using System.Collections.Generic;
using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    public class skill_level
    {
        [XmlAttribute]
        public int id { get; set; }
        [XmlAttribute]
        public string name { get; set; }
        [XmlAttribute]
        public int points { get; set; }
    }

    public class skill_levels
    {
        [XmlAttribute]
        public int total { get; set; }
        [XmlElement("skill_level")]
        public List<skill_level> skillLevelList { get; set; }
    }
}
