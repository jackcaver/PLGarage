using System.Xml.Serialization;

namespace GameServer.Models.Response
{
    [XmlType(TypeName = "game")]
    public class GameResponse
    {
        [XmlAttribute]
        public int id { get; set; }
        [XmlAttribute]
        public int game_player_id { get; set; }
        [XmlAttribute]
        public int game_player_stats_id { get; set; }
    }
}