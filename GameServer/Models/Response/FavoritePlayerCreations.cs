using System.Collections.Generic;
using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    public class favorite_player_creation 
    {
        [XmlAttribute]
        public int player_creation_id { get; set; }
        [XmlAttribute]
        public int player_creation_name { get; set; }
    }

    public class favorite_player_creations
    {
        [XmlAttribute]
        public int total { get; set; }
        public List<favorite_player_creation> PlayerCreations { get; set; }
    }
}