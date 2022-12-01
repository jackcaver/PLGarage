using System.Collections.Generic;
using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    public class review 
    {
        [XmlAttribute]
        public string content { get; set; }
        [XmlAttribute]
        public int id { get; set; }
        [XmlAttribute]
        public string mine { get; set; }
        [XmlAttribute]
        public int player_creation_id { get; set; }
        [XmlAttribute]
        public string player_creation_name { get; set; }
        [XmlAttribute]
        public string player_creation_username { get; set; }
        [XmlAttribute]
        public string player_creation_associated_item_ids { get; set; }
        [XmlAttribute]
        public string player_creation_level_mode { get; set; }
        [XmlAttribute]
        public string player_creation_is_team_pick { get; set; }
        [XmlAttribute]
        public int player_id { get; set; }
        [XmlAttribute]
        public string rated_by_me { get; set; }
        [XmlAttribute]
        public string rating_down { get; set; }
        [XmlAttribute]
        public string rating_up { get; set; }
        [XmlAttribute]
        public string username { get; set; }
        [XmlAttribute]
        public string tags { get; set; }
        [XmlAttribute]
        public string updated_at { get; set; }
    }

    public class reviews
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
        [XmlElement("review")]
        public List<review> ReviewList { get; set; }
    }
}