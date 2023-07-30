using System.Collections.Generic;
using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    [XmlType(TypeName = "tracks")]
    public class Tracks {
        [XmlAttribute]
        public int total { get; set; }
        [XmlElement("track")]
        public List<Track> TrackList { get; set; }
    }

    [XmlType(TypeName = "planet")]
    public class Planet
    {
        //[XmlAttribute]
        //public string auto_tags { get; set; }
        //[XmlAttribute]
        //public float coolness { get; set; }
        //[XmlAttribute]
        //public string created_at { get; set; }
        //[XmlAttribute]
        //public string description { get; set; }
        //[XmlAttribute]
        //public string dlc_keys { get; set; }
        //[XmlAttribute]
        //public int downloads { get; set; }
        //[XmlAttribute]
        //public int downloads_last_week { get; set; }
        //[XmlAttribute]
        //public int downloads_this_week { get; set; }
        //[XmlAttribute]
        //public string first_published { get; set; }
        //[XmlAttribute]
        //public int hearts { get; set; }
        [XmlAttribute]
        public int id { get; set; }
        //[XmlAttribute]
        //public bool is_remixable { get; set; }
        //[XmlAttribute]
        //public string last_published { get; set; }
        //[XmlAttribute]
        //public string moderation_status { get; set; }
        [XmlAttribute]
        public string name { get; set; }
        //[XmlAttribute]
        //public string platform { get; set; }
        //[XmlAttribute]
        //public string player_creation_type { get; set; }
        [XmlAttribute]
        public int player_id { get; set; }
        //[XmlAttribute]
        //public int rating_down { get; set; }
        //[XmlAttribute]
        //public int rating_up { get; set; }
        //[XmlAttribute]
        //public string tags { get; set; }
        //[XmlAttribute]
        //public string updated_at { get; set; }
        //[XmlAttribute]
        //public string user_tags { get; set; }
        [XmlAttribute]
        public string username { get; set; }
        //[XmlAttribute]
        //public int version { get; set; }
        //[XmlAttribute]
        //public int views { get; set; }
        //[XmlAttribute]
        //public int views_last_week { get; set; }
        //[XmlAttribute]
        //public int views_this_week { get; set; }
        public Tracks tracks { get; set; }
    }
}