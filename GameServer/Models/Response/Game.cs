using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    public class game
    {
        [XmlAttribute]
        public int id { get; set; }
        [XmlAttribute]
        public int game_player_id { get; set; }
        [XmlAttribute]
        public int game_player_stats_id { get; set; }
    }
}