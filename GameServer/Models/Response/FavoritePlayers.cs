using System.Collections.Generic;
using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    public class favorite_player 
    {
        [XmlAttribute]
        public int favorite_player_id { get; set; }
        [XmlAttribute]
        public int hearted_by_me { get; set; }
        [XmlAttribute]
        public int hearts { get; set; }
        [XmlAttribute]
        public int id { get; set; }
        [XmlAttribute]
        public string quote { get; set; }
        [XmlAttribute]
        public int total_tracks { get; set; }
        [XmlAttribute]
        public string username { get; set; }
    }

    public class favorite_players
    {
        [XmlAttribute]
        public int total { get; set; }
        public List<favorite_player> Players { get; set; }
    }
}