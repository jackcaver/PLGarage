using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    public class profile_acls
    {
        [XmlAttribute]
        public bool allow_all { get; set; }
        [XmlAttribute]
        public bool allow_psn { get; set; }
        [XmlAttribute]
        public bool deny_all { get; set; }
    }

    public class player_creation_acls
    {
        [XmlAttribute]
        public bool allow_all { get; set; }
        [XmlAttribute]
        public bool allow_psn { get; set; }
        [XmlAttribute]
        public bool deny_all { get; set; }
    }

    public class privacy_settings
    {
        public profile_acls profile_acls { get; set; }
        public player_creation_acls player_creation_acls { get; set; }
    }
}