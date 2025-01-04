using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    [XmlRoot("profile_acls")]
    public class ProfileAcls
    {
        [XmlAttribute("allow_all")]
        public bool AllowAll { get; set; }
        [XmlAttribute("allow_psn")]
        public bool AllowPsn { get; set; }
        [XmlAttribute("deny_all")]
        public bool DenyAll { get; set; }
    }

    [XmlRoot("player_creation_acls")]
    public class PlayerCreationAcls
    {
        [XmlAttribute("allow_all")]
        public bool AllowAll { get; set; }
        [XmlAttribute("allow_psn")]
        public bool AllowPsn { get; set; }
        [XmlAttribute("deny_all")]
        public bool DenyAll { get; set; }
    }

    [XmlRoot("privacy_settings")]
    public class PrivacySettings
    {
        [XmlElement("profile_acls")]
        public ProfileAcls ProfileAcls { get; set; }
        [XmlElement("player_creation_acls")]
        public PlayerCreationAcls PlayerCreationAcls { get; set; }
    }
}