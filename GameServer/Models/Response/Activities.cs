using System.Collections.Generic;
using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    [XmlType(TypeName = "event")]
    public class Event
    {
        [XmlAttribute]
        public int allusion_id { get; set; }
        [XmlAttribute]
        public string allusion_type { get; set; }
        [XmlAttribute]
        public int author_id { get; set; }
        [XmlAttribute]
        public string author_username { get; set; }
        [XmlAttribute]
        public int creator_id { get; set; }
        [XmlAttribute]
        public string creator_username { get; set; }
        [XmlAttribute]
        public string details { get; set; }
        [XmlAttribute]
        public int player_id { get; set; }
        [XmlAttribute]
        public int seconds_ago { get; set; }
        [XmlAttribute]
        public string tags { get; set; }
        [XmlAttribute]
        public string timestamp { get; set; }
        [XmlAttribute]
        public string topic { get; set; }
        [XmlAttribute]
        public string type { get; set; }
        [XmlAttribute]
        public string username { get; set; }
        [XmlAttribute]
        public string image_url { get; set; }
        [XmlAttribute]
        public string image_md5 { get; set; }
        [XmlAttribute]
        public string subject { get; set; }
    }

    public class activity
    {
        [XmlAttribute]
        public string player_creation_associated_item_ids { get; set; }
        [XmlAttribute]
        public string player_creation_description { get; set; }
        [XmlAttribute]
        public int player_creation_hearts { get; set; }
        [XmlAttribute]
        public int player_creation_id { get; set; }
        [XmlAttribute]
        public bool player_creation_is_team_pick { get; set; }
        [XmlAttribute]
        public int player_creation_level_mode { get; set; }
        [XmlAttribute]
        public string player_creation_name { get; set; }
        [XmlAttribute]
        public int player_creation_player_id { get; set; }
        [XmlAttribute]
        public int player_creation_races_started { get; set; }
        [XmlAttribute]
        public int player_creation_rating_down { get; set; }
        [XmlAttribute]
        public int player_creation_rating_up { get; set; }
        [XmlAttribute]
        public string player_creation_username { get; set; }
        [XmlAttribute]
        public int player_hearts { get; set; }
        [XmlAttribute]
        public int player_id { get; set; }
        [XmlAttribute]
        public string player_username { get; set; }
        [XmlAttribute]
        public string type { get; set; }
        public List<Event> events { get; set; }
    }

    public class activities
    {
        [XmlAttribute]
        public int page { get; set; }
        [XmlAttribute]
        public int row_end { get; set; }
        [XmlAttribute]
        public int row_start { get; set; }
        [XmlAttribute]
        public int total { get; set; }
        [XmlAttribute]
        public int total_pages { get; set; }
        [XmlElement("activity")]
        public List<activity> ActivityList { get; set; }
    }
}