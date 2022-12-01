using System.Collections.Generic;
using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    public class player_creation_comment 
    {
        [XmlAttribute]
        public string body { get; set; }
        [XmlAttribute]
        public string created_at { get; set; }
        [XmlAttribute]
        public int id { get; set; }
        [XmlAttribute]
        public string platform { get; set; }
        [XmlAttribute]
        public int player_creation_id { get; set; }
        [XmlAttribute]
        public int player_id { get; set; }
        [XmlAttribute]
        public string updated_at { get; set; }
        [XmlAttribute]
        public string username { get; set; }
        [XmlAttribute]
        public int rating_up { get; set; }
        [XmlAttribute]
        public int rating_down { get; set; }
        [XmlAttribute]
        public bool rated_by_me { get; set; }
    }

    public class player_creation_comments
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
        [XmlElement("player_creation_comment")]
        public List<player_comment> PlayerCreationCommentList { get; set; }
    }
}