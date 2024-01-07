using System.Collections.Generic;
using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    public class player_creation_rating 
    {
        [XmlAttribute]
        public string comments { get; set; }
        [XmlAttribute]
        public string rating { get; set; }
        //MNR
        [XmlAttribute]
        public string player_id { get; set; }
        [XmlAttribute]
        public string username { get; set; }
    }

    public class player_creation_ratings
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
        [XmlElement("player_creation_rating")]
        public List<player_creation_rating> PlayerCreationRatingList { get; set; }
    }
}