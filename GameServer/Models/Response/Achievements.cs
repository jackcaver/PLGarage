using System.Collections.Generic;
using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    [XmlType(TypeName = "achievement")]    // TODO: Can we change this to XmlRoot?
    public class Achievement
    {
        [XmlAttribute("achievement_type_id")]
        public int AchievementTypeId { get; set; }
        [XmlAttribute("created_at")]
        public string CreatedAt { get; set; }
        [XmlAttribute("has_read")]
        public bool HasRead { get; set; }
        [XmlAttribute("id")]
        public int Id { get; set; }
        [XmlAttribute("player_creation_id")]
        public string PlayerCreationId { get; set; }
        [XmlAttribute("player_creation_name")]
        public string PlayerCreationName { get; set; }
        [XmlAttribute("player_id")]
        public int PlayerId { get; set; }
        [XmlAttribute("updated_at")]
        public string UpdatedAt { get; set; }
        //MNR
        [XmlAttribute("relevant")]
        public bool Relevant { get; set; }
        [XmlAttribute("value")]
        public string Value { get; set; }
    }

    [XmlType(TypeName = "achievements")]    // TODO: Can we change this to XmlRoot?
    public class Achievements
    {
        [XmlAttribute("total")]
        public int Total { get; set; }
        [XmlElement("achievement")]
        public List<Achievement> AchievementList { get; set; }
    }
}